import React from 'react';
import { TouchableOpacity, View } from 'react-native';
import MyText from '@Components/MyText/MyText';
import MyImage from '@Components/MyImage/MyImage';
import ClickableItemStyles from './ClickableItemStyles';
import { isFunction } from "@Utilities/Methods";

const LINES_LIMIT = 3;
const ellipsizeMode = 'tail';

const ClickableItem = ({ name, description, image, price, clickFunc, icon }) => {
    const handleClick = () => {
        if(clickFunc && isFunction(clickFunc)) {
            clickFunc();
        }
    };

    const clickableItemStyles = ClickableItemStyles();
    
    return (
        <TouchableOpacity onPress={ handleClick }>
            <View style={ clickableItemStyles.container } >
                <MyImage 
                    style={ clickableItemStyles.image }
                    source={{uri: `${image ? 'data:image/png;base64,' + image : null }`}}
                />
                <View style={ clickableItemStyles.itemInfo }>
                    <View style={ clickableItemStyles.namePriceWrapper }>
                        <MyText style={ clickableItemStyles.name }>{ name }</MyText>
                        <MyText style={ clickableItemStyles.price }>{ price }</MyText>
                    </View>
                    <View style={ clickableItemStyles.descriptionWrapper }>
                        <MyText numberOfLines={ LINES_LIMIT } ellipsizeMode={ ellipsizeMode } style={ clickableItemStyles.description }>
                            { description ? description : null }
                        </MyText>
                    </View>
                </View>
                { icon && <View style={ clickableItemStyles.icon }>{ icon }</View> }
            </View>
        </TouchableOpacity>
    );
}

export default ClickableItem;