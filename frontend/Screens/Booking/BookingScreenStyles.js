import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const BookingScreenStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    slotsView: {
        height: '55%',
        padding: '3%',
        paddingBottom: '15%'
    },
    clickableColor: COLORS.primary,
    });
};

export default BookingScreenStyles;