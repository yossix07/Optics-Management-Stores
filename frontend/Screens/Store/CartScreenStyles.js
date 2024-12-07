import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const CartScreenStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    screenContainer: {
        justifyContent: 'space-between',
    },
    trashIcon: {
        color: COLORS.primary,
    },
    buyButton: {
        width: '95%',
    },
    });
};

export default CartScreenStyles;