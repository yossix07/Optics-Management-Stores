import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const MyDropDownStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    text: {
        color: COLORS.main_opposite,
    },
    dropDown: {
        alignSelf: 'center',
        width: '80%',
        marginTop:'3%',
        marginBottom:'3%',
        borderBottomColor: COLORS.secondary,
        borderBottomWidth: 0.5,
    },
    dropDownContainer: {
        backgroundColor: COLORS.main,
        borderColor: COLORS.main,
        borderWidth: 1,
        borderRadius: 5,
    },
    placeholder: {
        color: COLORS.main_opposite,
    },
    activeColor: COLORS.primary,
});
};

export default MyDropDownStyles;