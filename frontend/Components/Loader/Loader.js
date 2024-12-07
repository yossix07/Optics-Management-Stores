import React from "react";
import { View, ActivityIndicator } from "react-native";
import LoaderStyles from "./LoaderStyles";
import { LARGE } from "@Utilities/Constants";

const Loader = () => {
    const styles = LoaderStyles();
    return (
        <View style={ styles.loaderContainer }>
            <ActivityIndicator color={ styles.color } size={ LARGE }/>
        </View>
    );
};

export default Loader;