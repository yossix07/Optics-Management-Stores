import React from "react";
import { View } from "react-native";
import Modal from "react-native-modal";
import ColorPicker from 'react-native-wheel-color-picker';
import PressableButton from "@Components/PressableButton/PressableButton";
import { translate } from "@Utilities/translate";
import ColorModalStyles from "./ColorModalStyles";

const ColorModal = ({
        isModalVisible,
        currentColorOptions,
        currentColor,
        setCurrentColor,
        handleColorChange,
        handleColorConfirm,
        handleCancelCustomColors
    }) => {
    const styles = ColorModalStyles();
    return (
        <Modal isVisible={ isModalVisible }>
            <View style={ styles.modalConent }>
                <View style={ styles.colorTypes }>
                    {
                        currentColorOptions?.map((color, index) => 
                            <PressableButton 
                                key={ index }
                                style={{ opacity: index === currentColor ? 1 : 0.5 }}
                                onPressFunction={ () => setCurrentColor(index) }
                            >
                                { currentColorOptions[index].title }
                            </PressableButton>
                        )
                    }
                </View>
                <ColorPicker
                    onColorChangeComplete={ handleColorChange }
                    color={ currentColorOptions[currentColor].color }
                />
                <View style={ styles.actionsButtons }>
                    <PressableButton onPressFunction={ handleColorConfirm }>
                        { translate['save_changes'] }
                    </PressableButton>
                    <PressableButton onPressFunction={ handleCancelCustomColors }>
                        { translate['cancel'] }
                    </PressableButton>
                </View>
            </View>
        </Modal>
    );
};

export default ColorModal;