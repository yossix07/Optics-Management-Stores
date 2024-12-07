import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { MEDIUM_FONT_SIZE, BASE_MARGIN, BASE_PADDING } from '@Utilities/Styles';

const AppointmentSlotStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    alignItems: 'center',
    padding: 2 * BASE_PADDING,
    margin: BASE_MARGIN,
    backgroundColor: COLORS.primary,
    borderRadius: 5,

    textStyle: {
        color: COLORS.primary_opposite,
        fontWeight: 'bold',
        fontSize: MEDIUM_FONT_SIZE,
    }
    });
};

export default AppointmentSlotStyles;