import React, { useState, useContext } from "react";
import { StyleSheet, View } from "react-native";
import MyText from "@Components/MyText/MyText";
import CheckBox from 'expo-checkbox';
import PressableButton from "@Components/PressableButton/PressableButton";
import Input from "@Components/Input/Input";
import { api } from "@Services/API";
import { UserContext } from "@Contexts/UserContext";
import { translate } from "@Utilities/translate";
import GlobalStyles from "@Utilities/Styles";
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import LoginStyles from "./LoginStyles";
import { USER, TENANT, ERROR, SUCCESS } from '@Utilities/Constants';

const LoginScreen = ({ navigation }) => {
  const { logInFunction } = useContext(UserContext);
  const [isWorker, setIsWorker] = useState(false);
  const [formData, setFormData] = useState({
    id: '',
    password: '',
  });
  const { showLoader, hideLoader } = useLoader();
  const styles = LoginStyles();

  const handleLogin = () => {
    showLoader();
    if(isWorker) {
      api.tenantLogin(formData.id, formData.password, onTenantSuccsfulLogin, onErrorLogin);
    } else {
      api.userLogin(formData.id, formData.password, onUserSuccsfulLogin, onErrorLogin);
    }
  };

  const onUserSuccsfulLogin = (token) => {
    hideLoader();
    Toast.show({
      type: SUCCESS,
      text1: translate["welcome_back"],
  });
    logInFunction(formData.id, token, USER);
  }

  const onTenantSuccsfulLogin = (token) => {
    hideLoader();
    Toast.show({
      type: SUCCESS,
      text1: translate["welcome_back"],
    });
    logInFunction(formData.id, token, TENANT);
  }

  const onErrorLogin = (errorMessage) => {
    hideLoader();
    Toast.show({
      type: ERROR,
      text1: translate["something_went_wrong"],
      text2: errorMessage,
    });
  };
  
  const handleGoToSignup = () => {
    navigation.navigate('Sign-Up');
  };

  const handleGoToFogotPassword = () => {
    navigation.navigate('Forgot Password');
  };

  const handleInputChange = (name, value) => {
    setFormData((prevData) => ({ ...prevData, [name]: value }));
  };

  const globalStyles = GlobalStyles();

  return (
    <View style={ StyleSheet.compose(globalStyles?.container, styles?.center) }>
      <Input 
        placeholder={ translate['id_placeholder'] } 
        onChangeText={(text) => handleInputChange('id', text)} 
      />
      <Input 
        placeholder={ translate['password_placeholder'] }
        secureTextEntry={ true }
        onChangeText={ (text) => handleInputChange('password', text) }
      />
      <View style={ styles.checkboxContainer }>
        <MyText>{ translate["are_you_an_employee"] }</MyText>
        <CheckBox
          color={ styles.checkboxColor }
          value={ isWorker }
          onValueChange={ setIsWorker }
        />
      </View>
      <PressableButton onPressFunction={ handleLogin }>
        { translate["login"] }
      </PressableButton>
      <PressableButton onPressFunction={ handleGoToFogotPassword }>
        { translate["forgot_password"] }
      </PressableButton>
      <View style={ StyleSheet.compose(styles.row, styles.center) }>
        <MyText>
          { translate["dont_have_an_account"] }
        </MyText>
        <PressableButton onPressFunction={ handleGoToSignup }>
          { translate["signup"] }
        </PressableButton>
      </View>
    </View> 
  );
}

export default LoginScreen;