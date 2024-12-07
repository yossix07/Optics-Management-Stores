import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import { BASE_MARGIN } from '@Utilities/Styles';

const LoginStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
        center: {
            alignItems: "center",
            justifyContent: "center",
        },
        row: {
            flexDirection: "row",
        },
        checkboxContainer: {
            flexDirection: 'row',
            marginBottom: 4 * BASE_MARGIN,
        },
        checkbox: {
            alignSelf: 'center',
        },
        checkboxColor: COLORS.primary,
    });
};

export default LoginStyles;