import React, { useEffect, useState, useContext } from "react";
import Card from "@Components/Card/Card";
import { translate } from "@Utilities/translate";
import { api } from "@Services/API";
import { UserContext } from "@Contexts/UserContext";
import UserInfo from "@Components/UserInfo/UserInfo";
import ProductInfo from "@Components/ProductInfo/ProductInfo";
import BoxInfo from "@Components/BoxInfo/BoxInfo";
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import { useModal } from "@Hooks/UseModal";
import Icon from "@Components/Icon/Icon";
import { PENDING, READY, DELIVERD, CANCELED, ERROR, SUCCESS } from "@Utilities/Constants";
import OrderStyles from "./OrderStyles";

const Order = ({ order, onOrderStatusChange }) => {
    const { isTenant, token } = useContext(UserContext);
    const [ orderDate, setOrderDate ] = useState('');
    const [ statusButtons, setStatusButtons ] = useState([]);
    const { showLoader, hideLoader } = useLoader();
    const { showModal, hideModal } = useModal();
    const styles = OrderStyles();

    useEffect(() => {
        handleDate();
        if(isTenant()) {
            handleStatusButtons();
        }
    },[]);

    const handleDate = () => {
        const dateString = order.orderDate;
        const date = new Date(dateString);
        const formattedDate = date.toLocaleString("en-US", {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
            hour: "numeric",
            minute: "numeric",
            hour12: false,
            timeZone: "UTC"
        });
        setOrderDate(formattedDate);
    };

    const handleStatusChange = (status) => {
        showModal(
            translate["chage_status_message"],
            () => changeOrderStatus(status),
            hideModal,
        );
    };

    const handleStatusButtons = () => {
        switch (order.status) {
            case PENDING:
                setStatusButtons([
                    {
                        text: translate["set_order_ready"],
                        onPress: () => handleStatusChange(READY)
                    },
                    {
                        text: translate["set_order_delivered"],
                        onPress: () => handleStatusChange(DELIVERD)
                    },
                    {
                        text: <Icon title="trash" style={ styles.icon } />,
                        onPress: () => handleStatusChange(CANCELED)
                    }
                ])
                ;
                break;
            case READY:
                setStatusButtons([
                    {
                        text: translate["set_order_pending"],
                        onPress: () => handleStatusChange(PENDING)
                    },
                    {
                        text: translate["set_order_delivered"],
                        onPress: () => handleStatusChange(DELIVERD)
                    },
                    {
                        text: <Icon title="trash" style={ styles.icon }/>,
                        onPress: () => handleStatusChange(CANCELED)
                    }
                ]);
                break;
            case DELIVERD:
                setStatusButtons([
                    {
                        text: translate["set_order_pending"],
                        onPress: () => handleStatusChange(PENDING)
                    },
                    {
                        text: translate["set_order_ready"],
                        onPress: () => handleStatusChange(READY)
                    }
                ]);
                break;
        }
    };

    const changeOrderStatus = (status) => {
        showLoader();
        api.updateOrderStatus(order.id, status, token, handleSuccessFullReadyOrder, handleError);
    };

    const handleSuccessFullReadyOrder = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
        onOrderStatusChange(order.id);
    };

    const handleError = (error) => {
        hideLoader();
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error,
          });
    };

    return(
        <Card 
            title={ orderDate }
            icon="truck"
            titleButtons={ statusButtons }
        >
            {
                order.orderItems.map(item => {
                    return(
                        <ProductInfo key={ item.productId } product={ item }/>
                    );
                })
            }
            <TotalPrice price={ order.totalPrice }/>
            { isTenant() &&
                <UserInfo user={{
                    name: order.userName,
                    email: order.userEmail,
                    phone: order.userPhoneNumber
                }}/> 
            }
        </Card>
    );
};

export default Order;

const TotalPrice = ({ price }) => {
    return(
        <BoxInfo fields={[
            {
                icon: 'money',
                text: translate['total_price'] + ': ' + price
            }
        ]}/>
    );
};