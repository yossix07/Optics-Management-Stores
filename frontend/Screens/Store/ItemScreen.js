import React, { useState, useContext, useEffect, useCallback } from "react";
import { useFocusEffect } from "@react-navigation/native";
import { View, Text, ScrollView } from "react-native";
import { useRoute } from "@react-navigation/native";
import PressableButton from "@Components/PressableButton/PressableButton";
import Icon from "@Components/Icon/Icon";
import { CartContext } from "@Contexts/CartContext";
import MyImage from "@Components/MyImage/MyImage";
import { translate } from "@Utilities/translate";
import { UserContext } from "@Contexts/UserContext";
import { api } from "@Services/API";
import ItemScreenStyles from "./ItemScreenStyles";
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import { useColors } from "@Hooks/UseColors";
import { useModal } from "@Hooks/UseModal";
import BarChart from "@Components/BarChart/BarChart";
import Card from "@Components/Card/Card";
import { getMonthFirstDateXmonthsAgo, getMonthFirstDateXmonthsAhead } from "@Utilities/Date";
import { ERROR, SUCCESS, EDIT_MODE, STORE_ITEM } from "@Utilities/Constants";

const STATISTICS_MONTHS = 5;
const NEXT_MONTH = 1;

const ItemScreen = ({ navigation }) => {
    const { isUser, isTenant, token } = useContext(UserContext);
    const { addItemToCart } = useContext(CartContext);
    const [item, setItem] = useState({});
    const [soldUnitsData, setSoldUnitsData] = useState({});
    const [soldUnitsLabels, setSoldUnitsLabels] = useState([]);
    const [soldUnitsValues, setSoldUnitsValues] = useState([]);
    const route = useRoute();
    const itemId = route?.params?.itemId;
    const type = route?.params?.type;
    const { showLoader, hideLoader } = useLoader();
    const { showModal, hideModal } = useModal();

    const COLORS = useColors();

    useFocusEffect(
        useCallback(() => {
            if(itemId) {
                api?.getProductById(itemId, token, setItem, handleError);
            }
            if(isTenant()) {
                showLoader();
                const startDate = getMonthFirstDateXmonthsAgo(STATISTICS_MONTHS);
                const endDate = getMonthFirstDateXmonthsAhead(NEXT_MONTH);
                api?.getProductSales(itemId, startDate, endDate, token, handleSoldUnitsData, handleError);
            }
        },[])
    );

    useEffect(() => {
        parseBarChartData({ data: soldUnitsData, labelsSetter: setSoldUnitsLabels, valuesSetter: setSoldUnitsValues });
    },[soldUnitsData]);

    const handleSoldUnitsData = (data) => {
        hideLoader();
        setSoldUnitsData(data);
    };

    const parseBarChartData = ({ data, labelsSetter, valuesSetter}) => {
        const labels = [];
        const values = [];
        Object.keys(data).map((key) => {
            labels.push(key);
            values.push(data[key]);
        });
        labelsSetter(labels);
        valuesSetter(values);
    };

    const handleError = (error) => {
        hideLoader();
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error,
        });
    }

    const handleAddToCart = () => {
        if(item) {
            const isExist = addItemToCart(item);
            let message = translate["item_added_to_cart"];
            
            if(isExist) {
                message = translate["item_already_in_cart"];
            }
            showModal(
                message,
                () => { navigation.navigate('Store') },
                () => { navigation.replace('Cart') },
                translate["continue_shopping"],
                translate["go_to_cart"],
            );
        }
    }

    const handleEditItem = () => {
        navigation.navigate('Product-Form', { 
            mode: EDIT_MODE,
            item: item
        });
    }
    
    const handleRemoveItem = () => {
        showLoader();
        api.removeProduct(item.id, token, navigateBack, handleError);
    }

    const navigateBack = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
        navigation.goBack();
    }

    const activateModal = () => {
        showModal(
            translate["remove_confirm_message"],
            handleRemoveItem,
            hideModal
        );
    };

    const itemScreenStyles = ItemScreenStyles();

    return (
        <View style={ itemScreenStyles.container }>
            <ScrollView>
                <MyImage
                    style={ itemScreenStyles.image }
                    source={{uri: `${item.image ? 'data:image/png;base64,' + item.image : null }`}}
                />
                <View style={ itemScreenStyles.info }>
                    <View style={ itemScreenStyles.namePriceWrapper }>
                        <Text style={ itemScreenStyles.name }>{ item && item.name && item.name }</Text>
                        <Text style={ itemScreenStyles.price }>{ item && item.price && item.price }</Text>
                    </View>
                    <Text style={ itemScreenStyles.description } >{ item && item.description && item.description }</Text>
                </View>
                {
                    isTenant() &&
                    <Card
                        style={ itemScreenStyles.chartWrapper }
                        icon="box"
                        title={ translate["product_units_sold"] }
                        fitContent
                        > 
                            <View>
                                <BarChart
                                    labels={ soldUnitsLabels }
                                    data={ soldUnitsValues }
                                    isLastDiffrentColor
                                />
                            </View>
                        </Card>
                }
            </ScrollView>
                { isUser() && type === STORE_ITEM && 
                    <PressableButton style={ itemScreenStyles.addToCart } onPressFunction={ handleAddToCart }>
                        { translate["add_to_cart"] }
                    </PressableButton>
                }

                { isTenant() && type === STORE_ITEM && 
                    <View style={ itemScreenStyles.buttons } >
                        <PressableButton style={ itemScreenStyles.trashButton } onPressFunction={ handleEditItem }>
                            { translate["edit"] }
                        </PressableButton>
                        <PressableButton style={ itemScreenStyles.trashButton } onPressFunction={ activateModal }>
                            <Icon style={{ color: COLORS.primary_opposite }} title="trash"/>
                        </PressableButton>
                    </View>
                }
        </View>
    )
};

export default ItemScreen;