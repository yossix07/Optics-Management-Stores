import React, { useContext, createContext, useState, useEffect } from 'react';
import { api } from '@Services/API';
import { cartAPI as localStorgeCart } from '@Services/CartService';
import { UserContext } from '@Contexts/UserContext';

export const CartContext = createContext();

export function CartProvider({ children }) {
  const [ apiProdcuts, setApiProducts ] = useState([]);
  const [cartItems, setCartItems] = useState([]);
  const { username, token } = useContext(UserContext);

  // get cart items by username
  useEffect(() => {
    localStorgeCart.getItems(username).then((items) => {
      if(items) {
        setCartItems(items);
        api.getAllProducts(
          token,
          setApiProducts,
        );
      } else {
        localStorgeCart.setItems([]);
      }
    });
  },[username]);

  // update cart items when api products changed
  useEffect(() => {
    filterRemovedItems(apiProdcuts);
    editChangedItems(apiProdcuts);
  },[apiProdcuts]);

  // update cart items when api products removed
  const filterRemovedItems = (allItems) => {
    const removedItems = cartItems.filter((cartItem) => {
      const isExist = allItems.find((item) => item.id === cartItem.productId);
      return !isExist;
    });

    if(removedItems.length > 0) {
      setCartItems((prevItems) => {
        return prevItems.filter((prevItem) => {
          const isExist = allItems.find((item) => item.id === prevItem.productId);
          return isExist;
        });
      });
    }
  };

  // update cart items when api products changed
  const editChangedItems = (allItems) => {
    setCartItems((prevItems) => {
      return prevItems.map((prevItem) => {
        const isExist = allItems.find((item) => item.id === prevItem.productId);
        if(isExist) {
          return {
            ...prevItem,
            name: isExist.name,
            price: isExist.price,
            image: isExist.image,
            totalPrice: isExist.price * prevItem.quantity
          };
        }
      });
    });
  };

  // update cart items in local storge
  useEffect(() => {
    const cartItemsIds = cartItems.map((item) =>
     {
      return {
        productId: item.productId,
        quantity: item.quantity
      }
    });
    localStorgeCart.setItems(cartItemsIds, username);
  },[cartItems]);

  // returns true if item is in cart
  function isItemInCart(item) {
    return cartItems?.find(prevItem => prevItem.productId === item.productId || prevItem.productId === item.id);
  }

  // add item to cart
  function addItemToCart(item) {
    const isExist = isItemInCart(item);
    setCartItems((prevItems) => {
      // if item not exist in cart, add it as a new item
      if(!isExist) {
          return [...prevItems, {
            productId: item.id,
              name: item.name,
              quantity: 1,
              price: item.price,
              totalPrice: item.price,
              image: item.image
          }];
      }
      // if item exist in cart, increase quantity and total price
      else { 
          return prevItems.map((prevItem) => {
            if(prevItem.productId == item.productId) {
              prevItem.quantity++;
              prevItem.totalPrice += item.price;
            }
            return prevItem;
          });
      }
    });
    if(isExist) {
      return true;
    } 
    return false;
  }

  // substract item from cart
  function substractItemFromCart(item) {
    setCartItems((prevItems) => {
      const isExist = isItemInCart(item);
      if(isExist) {
        return prevItems.map((prevItem) => {
          if(prevItem.productId == item.productId && prevItem.quantity > 0) {
            prevItem.quantity--;
            prevItem.totalPrice -= item.price;
            if(prevItem.quantity == 0) {
              removeItemFromCart(prevItem);
            }
          }
          return prevItem;
        });
      }
      return prevItems;
    });
  }

  // remove item from cart
  function removeItemFromCart(item) {
    setCartItems((prevItems) => {
      const isExist = isItemInCart(item);
      if(isExist) {
        const itemIndex = prevItems.findIndex((prevItem) => prevItem.productId === item.productId);
        if(itemIndex !== -1) {
          prevItems.splice(itemIndex, 1);
          return [...prevItems];
        }
      }
      return prevItems;
    });
  }

  // empty cart
  function emptyCart() {
    setCartItems([]);
  };

  // get cart total price
  function getTotalPrice() {
    return cartItems.reduce((sum, item) => (sum + item.totalPrice), 0);
  }  

  return (
    <CartContext.Provider value={{ 
      cartItems,
      addItemToCart,
      substractItemFromCart,
      removeItemFromCart,
      getTotalPrice,
      emptyCart
    }}
    >
      { children }
    </CartContext.Provider>
  );
}