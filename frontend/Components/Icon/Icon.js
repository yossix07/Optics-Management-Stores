import React from 'react';
import { TouchableOpacity } from 'react-native';
import { FontAwesomeIcon } from '@fortawesome/react-native-fontawesome';
import { icons } from '@Utilities/Icons';
import { isFunction } from "@Utilities/Methods";

const Icon = ({ title, onPress, style }) => {
    if(isFunction(onPress)) {
        return (
            <TouchableOpacity onPress={ onPress } style={ style }>
                <FontAwesomeIcon icon={ icons[title] } style={ style }/>
            </TouchableOpacity>
        );
    } else {
        return (
            <FontAwesomeIcon icon={ icons[title] } style={ style }/>
        );
    }
};

export default Icon;