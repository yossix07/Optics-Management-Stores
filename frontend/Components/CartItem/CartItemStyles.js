import { StyleSheet } from 'react-native';
import { BASE_MARGIN } from '@Utilities/Styles';

const CartItemStyles = () => {
  
  return StyleSheet.create({
        quantityWrapper: {
            justifyContent: "center",
            alignItems: "center",
            marginTop: BASE_MARGIN,
        },
        counter: {
            marginTop: 2 * BASE_MARGIN,
        }
    });
};

export default CartItemStyles;