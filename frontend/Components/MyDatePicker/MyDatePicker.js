import React from "react";
import DatePicker from 'react-native-neat-date-picker';
import MyDatePickerStyles from "./MyDatePickerStyles";

export const RANGE_MODE = 'range';
export const SINGLE_MODE = 'single';

const MyDatePicker = ({ isVisible, mode=SINGLE_MODE, onCancel, onConfirm, chooseYearFirst=false }) => {
    const styles = MyDatePickerStyles();

    return (
        <DatePicker
            isVisible={ isVisible }
            mode={ mode }
            onCancel={ onCancel }
            onConfirm={ onConfirm }
            colorOptions={ styles.calendarTheme }
            chooseYearFirst={ chooseYearFirst }
        />
    );
};

export default MyDatePicker;