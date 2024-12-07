import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { BASE_MARGIN } from '@Utilities/Styles';
import { SMALL, MEDIUM, BIG } from '@Utilities/Constants';

const INPUT_HEIGHT = 45;

const InputStyles = (size) => {
  const COLORS = useColors();
  var width = 0;
  if(size === BIG) {
    width = '80%';
  } else if(size === MEDIUM) {
    width = '35%';
  } else if(size === SMALL) {
    width = '20%';
  } 
  
  return StyleSheet.create({
    inputView: {
        width: width ,
        height: INPUT_HEIGHT,
        alignItems: "center",
        borderRadius: 20,
        marginBottom: size === BIG ? 4 * BASE_MARGIN : 0,
        backgroundColor: COLORS.light_primary,
      },
      TextInput: {
        flex: 1,
        height: INPUT_HEIGHT,
        color: COLORS.primary_opposite,
      },
    });
};

export default InputStyles;