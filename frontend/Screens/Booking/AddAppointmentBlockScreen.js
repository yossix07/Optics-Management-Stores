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
import MyDropDown from "@Components/MyDropDown/MyDropDown";
import MyText from "@Components/MyText/MyText";
import AddScreenStyles from "./AddScreenStyles";
import { ERROR, SUCCESS, MEDIUM } from "@Utilities/Constants";

const AddAppointmentBlockScreen = ({ navigation }) => {
    const { token } = useContext(UserContext);
    const [selectedDay, setSelectedDay] = useState(0);
    const days = [
        { label: translate['sunday'], value: 0 },
        { label: translate['monday'], value: 1 },
        { label: translate['tuesday'], value: 2 },
        { label: translate['wednesday'], value: 3 },
        { label: translate['thursday'], value: 4 },
        { label: translate['friday'], value: 5 },
        { label: translate['saturday'], value: 6 },
    ];
    const [formData, setFormData] = useState({
        startHour: 0,
        startMinute: 0,
        endHour: 0,
        endMinute: 0,
    });
    const { showLoader, hideLoader } = useLoader();
    const styles = AddScreenStyles();

    const handleInputChange = (name, value) => {
        setFormData((prevData) => ({ ...prevData, [name]: value }));
    };

    const handleCreate = () => {
        showLoader();
        const args = {
            startHour: parseInt(formData.startHour),
            startMinute: parseInt(formData.startMinute),
            endHour: parseInt(formData.endHour),
            endMinute: parseInt(formData.endMinute),
            weekday: selectedDay
        };
        api.addAvaliableBlock(args, token, onCreateSuccess, handleError);
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
            <MyText style={ styles.title }> { translate["add_working_hours_title"] } </MyText>
            <MyDropDown
                items={ days }
                placeholder={ translate["select_day"]}
                value={ selectedDay }
                setValue={ setSelectedDay }
            />
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
            <PressableButton onPressFunction={ handleCreate }>
                { translate['create'] }
            </PressableButton>
        </View>
    );
};

export default AddAppointmentBlockScreen;