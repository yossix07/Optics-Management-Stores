import { StyleSheet } from 'react-native';
import { LARGE_FONT_SIZE } from '@Utilities/Styles';

const ROW_WIDTH = '80%';

const AddScreenStyles = () => {
  return StyleSheet.create({
    center: {
        alignItems: "center",
        justifyContent: "center",
    },
    title: {
        fontSize: LARGE_FONT_SIZE,
        fontWeight: "bold",
        marginBottom: '15%',
    },
    row: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        width: ROW_WIDTH,
        marginBottom: '5%',
    },
    datePickerRow: {
        width: ROW_WIDTH,
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: '5%',
    }
    });
};

export default AddScreenStyles;