import React from "react";
import { Text, View } from 'react-native';
import Modal from 'react-native-modal';
import ConfirmModalStyles from "./ConfirmModalStyles";
import PressableButton from "@Components/PressableButton/PressableButton";
import { translate } from "@Utilities/translate";
import { isFunction } from "@Utilities/Methods";

const ANIMATION_TIME = 0;
const NUM_OF_CENTER_BUTTONS = 1;

const ConfirmModal = ({
    text,
    isModalVisible,
    onApprove,
    apporveText=translate['approve'],
    onCancel,
    cancelText=translate['cancel'],
    closeModalFunc,
    animationInTiming=ANIMATION_TIME,
    animationOutTiming=ANIMATION_TIME
    }) => {
    const handleApprovePress = () => {
        
        if(isFunction(onApprove)) {
            onApprove();
        }

        if(isFunction(closeModalFunc)) {
            closeModalFunc();
        }
    };

    const handleCancelPress = () => {
        if(isFunction(onCancel)) {
            onCancel();
        }

        if(isFunction(closeModalFunc)) {
            closeModalFunc();
        }
    };
    let buttonsCounter = 0;
    if(onCancel) {
        buttonsCounter++;
    }
    if(onApprove) {
        buttonsCounter++;
    }

    // in case of 2 buttons, the buttons will be aligned to the sides
    const styles = ConfirmModalStyles(buttonsCounter === NUM_OF_CENTER_BUTTONS);

    return (
        <Modal isVisible={ isModalVisible } animationInTiming={ animationInTiming } animationOutTiming={ animationOutTiming }>
            <View style={ styles.modalContent }>
                <Text style={ styles.modalText }>
                    { text && text }
                </Text>
                <View style={ styles.modalButtons }>
                    <PressableButton onPressFunction={ handleApprovePress }>
                        { apporveText }
                    </PressableButton>
                    { onCancel && 
                        <PressableButton onPressFunction={ handleCancelPress }>
                            { cancelText }
                        </PressableButton>
                    }
                </View>
            </View>
        </Modal>
    );
};

export default ConfirmModal;