import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { BASE_MARGIN } from '@Utilities/Styles';

const BookingScreenStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
        center: {
            alignItems: "center",
            justifyContent: "center",
        },
        checkboxContainer: {
            flexDirection: 'row',
            marginBottom: 4 * BASE_MARGIN,
        },
        checkboxColor: COLORS.primary,
    });
};

export default BookingScreenStyles;