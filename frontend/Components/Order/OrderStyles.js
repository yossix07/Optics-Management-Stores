import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const OrderStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
        icon: {
            color: COLORS.primary,
        }
    });
};

export default OrderStyles;