import React from "react";
import { Text, StyleSheet } from "react-native";
import MyTextStyles from "./MyTextStyles";

const MyText = ({ style, children }) => {

    const textStyle = StyleSheet.compose(MyTextStyles(), style);

    return (
        <Text style={ textStyle } >
            { children }
        </Text>
    );
};

export default MyText;