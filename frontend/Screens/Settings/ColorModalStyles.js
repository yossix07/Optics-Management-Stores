import { StyleSheet } from 'react-native';

const ColorModalStyles = () => {
  return StyleSheet.create({
    modalConent: {
        flex: 1,
    },
    colorTypes: {
        flexDirection: 'row',
        justifyContent: 'space-evenly',
    },
    actionsButtons: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        marginTop: '10%'
    }
    });
};

export default ColorModalStyles;