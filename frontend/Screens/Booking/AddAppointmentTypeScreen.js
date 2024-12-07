import React, { useState, useContext } from "react";
import GlobalStyles from "@Utilities/Styles";
import { StyleSheet, View } from "react-native";
import Input from "@Components/Input/Input";
import PressableButton from "@Components/PressableButton/PressableButton";
import { api } from "@Services/API";
import { translate } from "@Utilities/translate";
import { UserContext } from "@Contexts/UserContext";
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import MyText from "@Components/MyText/MyText";
import AddScreenStyles from "./AddScreenStyles";
import { ERROR, SUCCESS } from "@Utilities/Constants";

const AddAppointmentTypeScreen = ({ navigation }) => {
    const { token } = useContext(UserContext);
    const [formData, setFormData] = useState({
        typeName: '',
        price: '',
    });
    const { showLoader, hideLoader } = useLoader();
    const styles = AddScreenStyles();

    const handleInputChange = (name, value) => {
        setFormData((prevData) => ({ ...prevData, [name]: value }));
    };

    const handleCreate = () => {
        showLoader();
        api.addAppointmentsType(formData, token, onCreateSuccess, handleError);
    };

    const onCreateSuccess = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
        navigation.navigate('Appointments-Settings');
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
            <MyText style={ styles.title }> { translate["add_appointment_type_title"] } </MyText>
            <Input
                placeholder={ translate['name_placeholder'] }
                onChangeText={(text) => handleInputChange('typeName', text)} 
            />
            <Input
                placeholder={ translate['price_placeholder'] }
                onChangeText={(text) => handleInputChange('price', text)}
            />
            <PressableButton onPressFunction={ handleCreate }>
                { translate['create'] }
            </PressableButton>
        </View>
    );
};

export default AddAppointmentTypeScreen;