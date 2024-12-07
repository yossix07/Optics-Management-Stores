import { StyleSheet } from 'react-native';
import { BASE_MARGIN } from '@Utilities/Styles';

const SignUpStyles = () => {
  
  return StyleSheet.create({
        container: {
            alignItems: "center",
            justifyContent: "center",
            paddingTop: '10%'
        },
        center: {
            alignItems: "center",
            justifyContent: "center",
        },
        row: {
            flexDirection: "row",
        },
        dateOfBirth: {
            alignItems: "center",
            width: '80%',
            marginBottom: 4 * BASE_MARGIN,
        },
    });
};

export default SignUpStyles;