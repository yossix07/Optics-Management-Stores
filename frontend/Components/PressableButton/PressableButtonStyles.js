import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { BASE_MARGIN, LARGE_FONT_SIZE } from '@Utilities/Styles';

const MIN_HEIGHT = 35;

const PressableButtonStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    touchableOpacityStyle: {
        minHeight: MIN_HEIGHT,
        margin: '2%',
        padding: '2%',
        borderRadius: 10,
        alignItems: "center",
        justifyContent: "center",
        backgroundColor: COLORS.primary,
    },
    buttonText: {
        color: COLORS.primary_opposite,
    },
    buttonContent: { 
      flexDirection: 'row',
      justifyContent: 'center',
      alignItems: 'center'
    },
    buttonIcon: {
        color: COLORS.primary_opposite,
        fontSize: LARGE_FONT_SIZE,
        marginRight: BASE_MARGIN
    },
    });
};

export default PressableButtonStyles;