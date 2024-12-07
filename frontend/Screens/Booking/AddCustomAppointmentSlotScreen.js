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
import { ERROR, SUCCESS, MEDIUM } from "@Utilities/Constants";
import MyDatePicker from "@Components/MyDatePicker/MyDatePicker";

const AddCustomAppointmentSlotScreen = ({ navigation }) => {
    const { token } = useContext(UserContext);
    const [showDatePicker, setShowDatePicker] = useState(false);
    const [formData, setFormData] = useState({
        startHour: 0,
        startMinute: 0,
        endHour: 0,
        endMinute: 0,
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
        const date = new Date(formData.year, formData.month - 1, formData.day);
        const dayOfWeek = date.getDay();
        const args = {
            date: {
                year: parseInt(formData.year),
                month: parseInt(formData.month),
                day: parseInt(formData.day),
            },
            appointmentsAvailableBlockDto: {
                startHour: parseInt(formData.startHour),
                startMinute: parseInt(formData.startMinute),
                endHour: parseInt(formData.endHour),
                endMinute: parseInt(formData.endMinute),
                weekDay: dayOfWeek
            }
        };
        api?.addCustomAppointmentSlot(args, token, onCreateSuccess, handleError);
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
            <MyDatePicker
                isVisible={ showDatePicker }
                onCancel={ cancelDatePicker }
                onConfirm={ onConfirmDatePick }
            />
            <MyText style={ styles.title }> { translate["add_custom_slot_title"] } </MyText>
            <View style={ styles.row }>
                <MyText> { translate["start_time_placeholder"] } </MyText>
                <Input
                    placeholder={ translate['start_hour_placeholder'] }
                    onChangeText={(text) => handleInputChange('startHour', text)}
                    size={ MEDIUM }
                />
                <Input
                    placeholder={ translate['start_minute_placeholder'] }
                    onChangeText={(text) => handleInputChange('startMinute', text)}
                    size={ MEDIUM }
                />
            </View>
            <View style={ styles.row }>
                <MyText> { translate["end_time_placeholder"] } </MyText>
                <Input
                    placeholder={ translate['end_hour_placeholder'] }
                    onChangeText={(text) => handleInputChange('endHour', text)}
                    size={ MEDIUM }
                />
                <Input
                    placeholder={ translate['end_minute_placeholder'] }
                    onChangeText={(text) => handleInputChange('endMinute', text)}
                    size={ MEDIUM }
                />
            </View>
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

export default AddCustomAppointmentSlotScreen;