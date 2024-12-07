import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const ToggleStyles = (isChecked) => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    trackColor: {
        false: COLORS.dark_primary,
        true: COLORS.primary
    },
    thumbColor: isChecked ? COLORS.light_primary : COLORS.grey
  });
};

export default ToggleStyles;