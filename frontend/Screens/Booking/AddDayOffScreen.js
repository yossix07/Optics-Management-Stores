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
import MyDatePicker from "@Components/MyDatePicker/MyDatePicker";

const AddDayOffScreen = ({ navigation }) => {
    const { token } = useContext(UserContext);
    const [showDatePicker, setShowDatePicker] = useState(false);
    const [formData, setFormData] = useState({
        name: '',
        day: '',
        month: '',
        year: '',
    });
    const { showLoader, hideLoader } = useLoader();
    const styles = AddScreenStyles();

    const handleInputChange = (name, value) => {
        setFormData((prevData) => ({ ...prevData, [name]: value }));
    };

    const handleCreate = () => {
        showLoader();
        const args = {
            name: formData.name,
            year: parseInt(formData.year),
            month: parseInt(formData.month),
            day: parseInt(formData.day),
        };
        api.addDayOff(args, token, onCreateSuccess, handleError);
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
        <View style={ StyleSheet.compose(globalStyles.container, styles.center) }>
            <MyText style={ styles.title }> { translate["add_day_off_title"] } </MyText>
            <MyDatePicker
                isVisible={ showDatePicker }
                onCancel={ cancelDatePicker }
                onConfirm={ onConfirmDatePick }
            />
            <Input
                placeholder={ translate['name_placeholder'] }
                onChangeText={(text) => handleInputChange('name', text)} 
            />
            <View style={ styles.datePickerRow }>
                <PressableButton onPressFunction={ openDatePicker } icon="calendar">
                    { translate['select_date'] }
                </PressableButton>
            </View>
            <PressableButton onPressFunction={ handleCreate }>
                { translate['create'] }
            </PressableButton>
        </View>
    );
};

export default AddDayOffScreen;