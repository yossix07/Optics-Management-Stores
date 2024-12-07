import React, { useState, useEffect } from "react";
import { View } from "react-native";
import ClickableItem from "@Components/ClickableItem/ClickableItem";
import Counter from "@Components/Counter/Counter";
import CartItemStyles from "./CartItemStyles";
import { isFunction } from "@Utilities/Methods";

// cart item is a clickable item with quantity control
const CartItem = ({ item, clickFunc, icon, addItemToCart, subtractItemFromCart, quantityControl=true }) => {
  const quantityIcon = quantityControl ? 
    <ItemQuantity
      key={ item.productId }
      item={ item }
      addItemToCart={ addItemToCart }
      subtractItemFromCart={ subtractItemFromCart }
      icon={ icon }
    /> : undefined;

  return (
    <ClickableItem
      name={ item?.name }
      image={ item?.image }
      price={ item?.price }
      clickFunc={ clickFunc }
      icon={ quantityIcon }
    />
  );
};

const ItemQuantity = ({ item, addItemToCart, subtractItemFromCart, icon }) => {
  const [quantity, setQuantity] = useState(item?.quantity);
  const styles = CartItemStyles();

  useEffect(() => {
    setQuantity(item?.quantity);
  }, []);

  const handleAdd = () => {
    if (isFunction(addItemToCart)) {
      addItemToCart(item);
    }
  };

  const handleSubtract = () => {
    if (isFunction(subtractItemFromCart)) {
      subtractItemFromCart(item);
    }
  };

  return (
    <View style={ styles.quantityWrapper }>
      { icon && icon }
      <Counter
        style={ styles.counter }
        initValue={ quantity }
        onAdd={ handleAdd }
        onSubtract={ handleSubtract }
      />
    </View>
  );
};

export default CartItem;