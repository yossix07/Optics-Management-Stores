
import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const GlobalStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    container: {
      height: '100%',
      backgroundColor: COLORS.main,
      color: COLORS.main_opposite,
    },
  });
};

export default GlobalStyles;

export const SMALL_FONT_SIZE = 14;
export const MEDIUM_FONT_SIZE = 16;
export const BIG_FONT_SIZE = 18;
export const LARGE_FONT_SIZE = 20;
export const EXTRA_LARGE_FONT_SIZE = 25;

export const BASE_MARGIN = 5;
export const BASE_PADDING = 5;