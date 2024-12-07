import React from "react";
import MyDropDownStyles from "./MyDropDownStyles";
import { Dropdown } from 'react-native-element-dropdown';

const MAX_HEIGHT = 300;

const MyDropDown = ({ value, setValue, items, placeholder }) => {
    const styles = MyDropDownStyles();
    return(
        <Dropdown
            style={ styles.dropDown }
            activeColor={ styles.activeColor }
            containerStyle={ styles.dropDownContainer }
            itemTextStyle={ styles.text }
            selectedTextStyle={ styles.text }
            data={ items }
            maxHeight={ MAX_HEIGHT }
            labelField="label"
            valueField="value"
            placeholder={ placeholder }
            placeholderStyle={ styles.placeholder }
            value={ value }
            onChange={ item => {
                setValue(item.value);
            }}
        />
    );
};

export default MyDropDown;