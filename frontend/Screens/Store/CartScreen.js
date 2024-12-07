import React, { useContext } from "react";
import { View, StyleSheet, ScrollView } from "react-native";
import GlobalStyles from "@Utilities/Styles";
import CartItem from "@Components/CartItem/CartItem";
import { CartContext } from "@Contexts/CartContext";
import Icon from "@Components/Icon/Icon";
import { api } from '@Services/API';
import PressableButton from '@Components/PressableButton/PressableButton';
import { translate } from "@Utilities/translate";
import { UserContext } from "@Contexts/UserContext";
import CartScreenStyles from "./CartScreenStyles";
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import { useModal } from "@Hooks/UseModal";
import { ERROR, SUCCESS, CART_ITEM } from "@Utilities/Constants";

const CartScreen = ({ navigation }) => {
    const { username, token } = useContext(UserContext);
    const { 
        cartItems,
        addItemToCart,
        substractItemFromCart,
        removeItemFromCart,
        emptyCart,
        getTotalPrice
    } = useContext(CartContext);
    const { showLoader, hideLoader } = useLoader();
    const { showModal, hideModal } = useModal();


    const handleItemClick = (item) => {
        api.getProductById(
            item.productId,
            token,
            () => { navigateToItemScreen(item) },
            handleError
        )
    };

    const navigateToItemScreen = (item) => {
        navigation.navigate('Item',  {
            itemId: item?.productId,
            type: CART_ITEM
        });
    };

    // send order to server
    const handleOrder = () => {
        showLoader();
        const cartItemsWithProductName = cartItems.map((item) => {
            return {
                ...item,
                ProductName: item.name
            }
        });
        api.createOrder(
            username,
            cartItemsWithProductName,
            token,
            onSuccessOrder,
            handleError
        );
    };

    const onSuccessOrder = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
        emptyCart();
        navigation.navigate('Store');
    };

    const handleError = (error) => {
        hideLoader();
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error,
          });
    };

    // request confirmation from user
    const activateModal = () => {
        showModal(
            translate["order_confirm_message"],
            handleOrder,
            hideModal
        );
    };

    const styles = CartScreenStyles();
    const globalStyles = GlobalStyles();

    return (
        <View style={ StyleSheet.compose(globalStyles.container, styles.screenContainer) }>
            <ScrollView style={ styles.cartItems }>
                { cartItems && cartItems.map((item, index) => (
                    <CartItem
                        key={ index }
                        item={ item }
                        clickFunc={ () => { handleItemClick(item) }}
                        icon={<Icon title={ 'trash' } style={ styles.trashIcon }
                        onPress= {() => { removeItemFromCart(item) }}/>}
                        addItemToCart={ addItemToCart }
                        subtractItemFromCart={ substractItemFromCart }/>
                ))}
            </ScrollView>
            <PressableButton style={ styles.buyButton } onPressFunction={ activateModal }>
                { translate["buy_now"] }
                { getTotalPrice() }
            </PressableButton>
        </View>
    );
};

export default CartScreen;