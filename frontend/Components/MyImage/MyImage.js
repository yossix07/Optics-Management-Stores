import React from "react";
import { Image } from "react-native";
import FallbackImage from "./FallbackImage";

const NULL = "null";

const myImage = ({ style, source }) => {
    if (source && source.uri && source.uri !== NULL) {
        return (
            <Image style={ style } source={ source }/>
        );
    }
    return (
        <FallbackImage style={ style }/>
    );
};

export default myImage;