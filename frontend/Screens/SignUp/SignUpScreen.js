import React, { useState } from "react";
import { StyleSheet, View, ScrollView } from "react-native";
import MyText from "@Components/MyText/MyText";
import PressableButton from "@Components/PressableButton/PressableButton";
import Input from "@Components/Input/Input";
import { api } from "@Services/API";
import { translate } from "@Utilities/translate";
import GlobalStyles from "@Utilities/Styles";
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import SignUpStyles from "./SignUpStyles";
import { ERROR, SUCCESS } from "@Utilities/Constants";
import MyDatePicker from "@Components/MyDatePicker/MyDatePicker";

const SignupScreen = ({ navigation }) => {
  const [formData, setFormData] = useState({
    id: '',
    name: '',
    email: '',
    password: '',
    verifyPassword: '',
    phoneNumber: '',
    day: '',
    month: '',
    year: '',
  });
  const { showLoader, hideLoader } = useLoader();
  const [showDatePicker, setShowDatePicker] = useState(false);

  const styles = SignUpStyles();

  const handleInputChange = (name, value) => {
    setFormData((prevData) => ({ ...prevData, [name]: value }));
  };

  const handleSignup = () => {
    showLoader();
    if(formData.password === formData.verifyPassword) {
      const date = `${formData.year}-${formData.month}-${formData.day}`
      api.userSignup(formData.id, formData.name, formData.password, formData.email, 
                 formData.phoneNumber, date, onSuccsfulSignup, onErrorSignup)
    } else {
      onErrorSignup(translate["passwords_dont_match"]);
    }
  };

  const handleGoToLogin = () => {
    navigation.navigate('Login');
  };
  
  onSuccsfulSignup = () => {
    hideLoader();
    Toast.show({
      type: SUCCESS,
      text1: translate["action_success"],
    });
    handleGoToLogin();
  }

  onErrorSignup = (error) => {
    hideLoader();
    Toast.show({
      type: ERROR,
      text1: translate["something_went_wrong"],
      text2: error,
    });
  }

  const openDatePicker = () => setShowDatePicker(true);

  const cancelDatePicker = () => setShowDatePicker(false);

  const onConfirmDatePick = (output) => {
    setShowDatePicker(false)
    const [ pickedYear, pickedMonth, pickedDay ] = output.dateString.split('-')
    setFormData({
        ...formData,
        day: pickedDay,
        month: pickedMonth,
        year: pickedYear
    });
  };

  const globalStyles = GlobalStyles();

  return (
      <View style={ globalStyles.container }>
        <MyDatePicker
          isVisible={ showDatePicker }
          onCancel={ cancelDatePicker }
          onConfirm={ onConfirmDatePick }
          chooseYearFirst={ true }
        />
        <ScrollView style={ globalStyles.container } contentContainerStyle={ styles.container }>
        <Input 
          placeholder={ translate['id_placeholder'] } 
          onChangeText={(text) => handleInputChange('id', text)} 
        />
        <Input 
          placeholder={ translate['email_placeholder'] }
          onChangeText={(text) => handleInputChange('email', text)}
        />
        <Input 
          placeholder={ translate['name_placeholder'] }
          onChangeText={(text) => handleInputChange('name', text)}
        />
        <Input 
          placeholder={ translate['password_placeholder'] }
          secureTextEntry={ true }
          onChangeText={ (text) => handleInputChange('password', text) }
        />
        <Input 
          placeholder={ translate['verify_password_placeholder'] }
          secureTextEntry={ true }
          onChangeText={ (text) => handleInputChange('verifyPassword', text) }
        />
        <Input 
          placeholder={ translate['phone_number_placeholder'] }
          onChangeText={ (text) => handleInputChange('phoneNumber', text) }
        />
        <View style={ styles.dateOfBirth }>
          <PressableButton onPressFunction={ openDatePicker } icon="calendar"> 
            { translate["pick_date_of_birth"] }
          </PressableButton>
        </View>        
        <PressableButton onPressFunction={ handleSignup }> { translate["signup"] } </PressableButton>
        <View style={ StyleSheet.compose(styles.row, styles.center) }>
          <MyText> { translate["already_signed"] } </MyText>
          <PressableButton onPressFunction={ handleGoToLogin }> { translate["login"] } </PressableButton>
        </View>
      </ScrollView> 
      </View>
  );
};

export default SignupScreen;