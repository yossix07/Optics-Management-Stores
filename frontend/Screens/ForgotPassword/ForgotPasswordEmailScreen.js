import React, { useState } from "react";
import GlobalStyles from "@Utilities/Styles";
import { StyleSheet, View } from "react-native";
import Input from "@Components/Input/Input";
import PressableButton from "@Components/PressableButton/PressableButton";
import { api } from "@Services/API";
import { translate } from "@Utilities/translate";
import Toast from 'react-native-toast-message';
import MyText from "@Components/MyText/MyText";
import CheckBox from 'expo-checkbox';
import { useLoader } from "@Hooks/UseLoader";
import ForgotPasswordStyles from "./ForgotPasswordStyles";
import { USER, TENANT } from "@Utilities/Constants";
import { ERROR, SUCCESS } from "@Utilities/Constants";

const ForgotPasswordEmailScreen = ({ navigation }) => {
    const [email, setEmail] = useState('');
    const [isWorker, setIsWorker] = useState(false);
    const { showLoader, hideLoader } = useLoader();
    const styles = ForgotPasswordStyles();

    const handleEmailChange = (text) => {
        setEmail(text);
    };

    const handleSendEmail = () => {
        showLoader();
        const role = isWorker ? TENANT : USER;
        api.sendResetPasswordEmail(email, role, onSendEmailSuccess, handleError);
    };

    const onSendEmailSuccess = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
        navigation.navigate('Reset-Password', {
            email: email,
        });
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
                placeholder={translate['email_placeholder']}
                onChangeText={ handleEmailChange }
            />
            <View style={ styles.checkboxContainer }>
                <MyText>
                    { translate["are_you_an_employee"] }
                </MyText>
                <CheckBox
                    color={ styles.checkboxColor }
                    value={ isWorker }
                    onValueChange={ setIsWorker }
                />
            </View>
            <PressableButton onPressFunction={ handleSendEmail }>
                { translate['send_email_button'] }
            </PressableButton>
        </View>
    );
};

export default ForgotPasswordEmailScreen;