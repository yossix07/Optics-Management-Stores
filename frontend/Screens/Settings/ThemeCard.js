import React, { useContext, useState, useEffect, useRef } from 'react';
import { View } from 'react-native';
import { translate } from '@Utilities/translate';
import PressableButton from '@Components/PressableButton/PressableButton';
import Card from '@Components/Card/Card';
import { themes, CUSTOM } from '@Utilities/Constants';
import SettingsScreenStyles from './SettingsScreenStyles';
import { ThemeContext } from '@Contexts/ThemeContext';
import ColorModal from './ColorModal';

const SELECTED_OPACITY = 1;
const UNSELECTED_OPACITY = 0.5;
const THEME_BUTTON_WIDTH = '30%';

const ThemeCard = () => {
    const { theme, setTheme, customColors, setCustomTheme } = useContext(ThemeContext);
    const [isModalVisible, setModalVisible] = useState(false);
    const originalMainColor = useRef(customColors.main);
    const originalPrimaryColor = useRef(customColors.primary);
    const originalSecondaryColor = useRef(customColors.secondary);
    const [selectedMainColor, setSelectedMainColor] = useState(customColors.main);
    const [selectedPrimaryColor, setSelectedPrimaryColor] = useState(customColors.primary);
    const [selectedSecondaryColor, setSelectedSecondaryColor] = useState(customColors.secondary);
    const [currentColor, setCurrentColor] = useState(0);
    const currentColorOptions = [
        {
            color: selectedMainColor,
            setter: setSelectedMainColor,
            title: translate['background_color']
        },
        {
            color: selectedPrimaryColor,
            setter: setSelectedPrimaryColor,
            title: translate['buttons_color']
        },
        {
            color: selectedSecondaryColor,
            setter: setSelectedSecondaryColor,
            title: translate['icons_color']
        },
    ];

    useEffect(() => {
        setCustomTheme(selectedMainColor, selectedPrimaryColor, selectedSecondaryColor);
    }, [selectedMainColor, selectedPrimaryColor, selectedSecondaryColor]);

    const styles = SettingsScreenStyles();

    const handleCusomizeColors = () => {
        setTheme(CUSTOM);
        setModalVisible(true);
    };

    const handleColorChange = (color) => {
        currentColorOptions[currentColor].setter(color);
    };

    const handleColorConfirm = () => {
        setModalVisible(false);
    };
  
    const handleCancelCustomColors = () => {
        setSelectedMainColor(originalMainColor.current);
        setSelectedPrimaryColor(originalPrimaryColor.current);
        setSelectedSecondaryColor(originalSecondaryColor.current);
        setModalVisible(false);
    };

    return(
        <Card title={ translate["theme_card_title"] } icon="paint" small={ true }>
            <View style={ styles.themeContainer }>
                {
                    themes?.map((themeItem, index) => {
                        return(
                            <PressableButton 
                                key={ index }
                                icon={ themeItem }
                                style={{
                                    opacity: theme === themeItem ? SELECTED_OPACITY : UNSELECTED_OPACITY,
                                    width: THEME_BUTTON_WIDTH,
                                }}
                                onPressFunction={ () => setTheme(themeItem) }
                            >
                                { translate[themeItem] }
                            </PressableButton>
                        )
                    })
                }
            </View>
            <PressableButton onPressFunction={ handleCusomizeColors } icon="sliders">
                { translate['customize_colors'] }
            </PressableButton>
            <ColorModal
                isModalVisible={ isModalVisible }
                currentColorOptions={ currentColorOptions }
                currentColor={ currentColor }
                setCurrentColor={ setCurrentColor }
                handleColorChange={ handleColorChange }
                handleColorConfirm={ handleColorConfirm }
                handleCancelCustomColors={ handleCancelCustomColors }
            /> 
        </Card>
    );
};

export default ThemeCard;