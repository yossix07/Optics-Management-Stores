import React from "react";
import { TouchableOpacity, Text } from "react-native";
import AppointmentSlotStyles from "./AppointmentSlotStyles";

// clickable appointment slot
const AppointmentSlot = ({ item, onPress }) => {
    const styles = AppointmentSlotStyles();
    
    return (
    <TouchableOpacity style={ styles } onPress={ () => { onPress(item) } }>
        <Text style={ styles.textStyle }>
            { item.startTime }
        </Text>
    </TouchableOpacity>
    );
};

export default AppointmentSlot;
