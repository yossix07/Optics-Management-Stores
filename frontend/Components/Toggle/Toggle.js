import React from "react";
import { StyleSheet } from "react-native";
import { Switch } from "react-native-elements";
import { isFunction } from "@Utilities/Methods";
import ToggleStyles from "./ToggleStyles";

const Toggle = ({ style, value, onValueChange, trueValue }) => {
    const styles = ToggleStyles(value == trueValue);
    const handleChange = (val) => {
        if(isFunction(onValueChange)) {
            onValueChange(val);
        }
    };

    return(
        <Switch
            style={ StyleSheet.compose(styles, style) }
            trackColor={ styles.trackColor }
            thumbColor={ styles.thumbColor }
            onValueChange={ handleChange }
            value={ value == trueValue }/>
    );
};

export default Toggle;