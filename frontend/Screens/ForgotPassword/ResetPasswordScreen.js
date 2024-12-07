import React, { useState } from "react";
import GlobalStyles from "@Utilities/Styles";
import { StyleSheet, View } from "react-native";
import Input from "@Components/Input/Input";
import PressableButton from "@Components/PressableButton/PressableButton";
import { api } from "@Services/API";
import { translate } from "@Utilities/translate";
import Toast from 'react-native-toast-message';
import { useRoute } from "@react-navigation/native";
import { useLoader } from "@Hooks/UseLoader";
import ForgotPasswordStyles from "./ForgotPasswordStyles";
import { ERROR, SUCCESS } from "@Utilities/Constants";

const ResetPasswordScreen = ({ navigation }) => {
    const [formData, setFormData] = useState({
        verificationCode: '',
        newPassword: '',
        passwordValidation: '',
    });
    const { showLoader, hideLoader } = useLoader();
    const styles = ForgotPasswordStyles();

    const route = useRoute();
    const { email } = route.params;

    const handleInputChange = (name, value) => {
        setFormData((prevData) => ({ ...prevData, [name]: value }));
      };

    const handleRestPassword = () => {
        showLoader();
        formData.email = email;
        api.resetPassword(formData, onResetSuccess, handleError);
    };

    const onResetSuccess = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
        navigation.navigate('Login');
    };

    const handleError = (error) => {
        hideLoader();
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error,
        });
    };

    const globalStyles = GlobalStyles();

    return (
        <View style={ StyleSheet.compose(globalStyles.container, styles.center) }>
            <Input
                placeholder={ translate['pincode_placeholder'] }
                onChangeText={(text) => handleInputChange('verificationCode', text)} 
            />
            <Input
                placeholder={ translate['new_password_placeholder'] }
                onChangeText={(text) => handleInputChange('newPassword', text)}
                secureTextEntry={ true }
            />
            <Input
                placeholder={ translate['verify_new_password_placeholder'] }
                onChangeText={(text) => handleInputChange('passwordValidation', text)}
                secureTextEntry={ true }
            />
            <PressableButton onPressFunction={ handleRestPassword }>
                { translate['reset_password'] }
            </PressableButton>
        </View>
    );
};

export default ResetPasswordScreen;