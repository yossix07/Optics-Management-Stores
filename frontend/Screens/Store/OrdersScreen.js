import React, { useEffect, useContext, useState } from "react";
import { View, ScrollView } from "react-native";
import GlobalStyles from "@Utilities/Styles";
import { api } from "@Services/API";
import { UserContext } from "@Contexts/UserContext";
import MyDropDown from "@Components/MyDropDown/MyDropDown";
import Order from "@Components/Order/Order";
import Toast from 'react-native-toast-message';
import { PENDING, READY, DELIVERD, CANCELED } from "@Utilities/Constants";
import { useLoader } from "@Hooks/UseLoader";
import { ERROR } from "@Utilities/Constants";

const OrdersScreen = () => {
    const { username, token, isUser, isTenant } = useContext(UserContext);
    const [orders, setOrders] = useState([]);
    const [status, setStatus] = useState(PENDING);
    const { showLoader, hideLoader } = useLoader();
    const statuses = [
        { label: PENDING, value: PENDING },
        { label: READY, value: READY },
        { label: DELIVERD, value: DELIVERD },
        { label: CANCELED, value: CANCELED }
    ];

    const handleOrdersData = (data) => {
        hideLoader();
        setOrders(data);
    };

    useEffect(() => {
        if (isTenant()) {
            setStatus(PENDING);
            api.getOrdersByStatus(PENDING, token, handleOrdersData, handleError);
        } else if (isUser()) {
            api.getOrdersByUserId(username, PENDING, token, handleOrdersData, handleError);
        }
    },[]);

    useEffect(() => {
        showLoader();
        if(isTenant()) {
            api.getOrdersByStatus(status, token, handleOrdersData, handleError);
        } else if (isUser()) {
            api.getOrdersByUserId(username, status, token, handleOrdersData, handleError);
        }
    },[status]);

    const handleError = (error) => {
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error,
          });
    };

    const onOrderStatusChange = (orderId) => {
        setOrders(prev => prev.filter(order => order.id !== orderId));
    };

    const globalStyles = GlobalStyles();

    return (
        <View style={ globalStyles.container }>
            <MyDropDown
                value={ status }
                items={ statuses }
                setValue={ setStatus }
            />
                <ScrollView nestedScrollEnabled = { true }>
                    { 
                        orders?.map(order =>
                            <Order
                                key={ order.id }
                                order={ order }
                                onOrderStatusChange={ onOrderStatusChange }
                            />    
                        )
                    }
                </ScrollView>
        </View>
    );
};

export default OrdersScreen;