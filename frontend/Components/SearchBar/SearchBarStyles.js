import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const SearchBarStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    containerStyle: {
        backgroundColor: COLORS.main,

    }, 
    inputContainerStyle: {
        backgroundColor: COLORS.main,
        color: COLORS.main_opposite,
    },
    searchIcon: {
        color: COLORS.secondary,
    }
  });
};

export default SearchBarStyles;