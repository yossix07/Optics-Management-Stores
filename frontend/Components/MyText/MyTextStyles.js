import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const MyTextStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    color: COLORS.main_opposite,
  });
};

export default MyTextStyles;