import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { BIG_FONT_SIZE } from '@Utilities/Styles';

const ConfirmModalStyles = (centerButtons) => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    modalContent: {
        justifyContent: 'center',
        alignItems: 'center',
        paddingHorizontal: '1%',
        paddingVertical: '5%',
        borderRadius: 8,
        backgroundColor: COLORS.light_primary,
    },
    modalText: {
        marginBottom: '10%',
        fontSize: BIG_FONT_SIZE,
        fontWeight: 'bold',
        color: COLORS.primary_opposite,
    },
    modalButtons: {
        flexDirection: 'row',
        justifyContent: centerButtons ? 'center' : 'space-between',
        width: '100%',
    },
    });
};

export default ConfirmModalStyles;