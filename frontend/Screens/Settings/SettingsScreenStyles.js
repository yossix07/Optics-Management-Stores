import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const SettingsScreenStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    themeContainer: {
        flexDirection: 'row',
        justifyContent: 'space-evenly',
    },
    themeButtonIcon: {
        color: COLORS.primary_opposite,
    },
    themeButtonText: {
        color: COLORS.primary_opposite,
    }
    });
};

export default SettingsScreenStyles;