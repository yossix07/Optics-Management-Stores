import React, { useContext, useState, useCallback, useEffect} from "react";
import GlobalStyles from "@Utilities/Styles";
import { View, ScrollView } from "react-native";
import { UserContext } from "@Contexts/UserContext";
import Cards from "@Components/Cards/Cards";
import { api } from '@Services/API';
import { translate } from "@Utilities/translate";
import { useFocusEffect } from "@react-navigation/native";
import { getCurrentDate, getCurrentMonthLastDate } from "@Utilities/Date";
import BoxInfo from "@Components/BoxInfo/BoxInfo";
import { CartContext } from "@Contexts/CartContext";
import ProductInfo from "@Components/ProductInfo/ProductInfo";
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import { statuses } from "@Utilities/Constants";
import { useModal } from "@Hooks/UseModal";
import { SUCCESS, ERROR, LIST_TYPE, DICT_TYPE } from "@Utilities/Constants";

const API_CALLS = 4;

const HomeScreen = ({ navigation }) => {
  const { username, isUser, isTenant, token } = useContext(UserContext);
  const { cartItems } = useContext(CartContext);
  const [appointments, setAppointments] = useState([]);
  const [toBeRemoved, setToBeRemoved] = useState('');
  const [cardsData, setCardsData] = useState([]);
  const [orders, setOrders] = useState([]);
  const [ dataLoaderCount, setDataLoaderCount ] = useState(0);
  const { showLoader, hideLoader } = useLoader();
  const { showModal, hideModal } = useModal();
  const globalStyles = GlobalStyles();

  useEffect(() => {
    initCardsData();

    if(isTenant()) {
      initTenantCardsData();
    }

    else if(isUser()) {
      initUserCardsData();
    }
  },[]);

  const initCardsData = () => {
    setCardsData(
      [
        {
          title: translate["appointment_card_title"],
          icon: 'calendar',
          titleButtons: [
            { text: translate["appointment_button_title"], onPress: handleGoToBooking }
          ],
          dict: appointments,
          renderItem: (keyAndItem) => {
            const [key, item] = keyAndItem;
            const remove = () => {
              showLoader();
              setToBeRemoved(item);
            };
            return(
              <BoxInfo key={ item.id } fields={[
                  { 
                    icon: 'clock',
                    text: `${key} : ${item.startTime}`,
                    removable: true,
                    removeFunction: () => {
                      showModal(
                        translate["remove_appointment_message"],
                        remove,
                        hideModal,
                      );
                    }
                  },
                ]}
              />
            )
          },
        },
        {
          title: translate["orders_card_title"],
          icon: 'truck',
          titleButtons: [
            { text: translate["orders_button_title"], onPress: handleGoToOrders }
          ],
          dict: orders,
          renderItem: (keyAndItem) => {
            const [key, item] = keyAndItem;
            return(
              <BoxInfo key={ `${key} : ${item}` } fields={[
                  { text: `${key} : ${item}`, icon: `${key.toLowerCase()}`  }
                ]}
              />
            )
          },
        },
      ]
    );
  };

  const initTenantCardsData = () => {
    setCardsData(prev => {
      const updated = prev.map(card => {
        if(card.title === translate["appointment_card_title"]) {
          return {
            ...card,
            titleButtons: [
              { text: translate["appoinments_settings_title"] , onPress: handleGoToAppointmentsSettings },
              ...card.titleButtons
            ]
          }
        }
        if(card.title === translate["orders_card_title"]) {
          return {
            ...card,
            titleButtons: [
              { text: translate["shopping_button_title"], onPress: handleGoToStore },
              ...card.titleButtons
            ]
          }
        }
        return card;
      });
      return updated;
    });
  };

  const initUserCardsData = () => {
    setCardsData(prev => {
      return(
        [
          ...prev,
          {
            title: translate["shopping_card_title"],
            icon: "cart",
            titleButtons: [
              { text: translate["shopping_cart_button_title"], onPress: handleGoToCart },
              { text: translate["shopping_button_title"], onPress: handleGoToStore }
            ],
            list: cartItems,
            renderItem: (item) => {
              if(item && item.name && item.price && item.quantity){
                return(
                  <ProductInfo key={ `${item.productId}` } product={ item }/>
                )
              }
            }
          }
        ]
      )
    });
  };

  const updateCardData = useCallback((cardTitle, newData) => {
    setCardsData(prev => {
      const updated = prev.map(card => {
        if (card.title === translate[cardTitle]) {
          return { ...card, [newData.type]: newData.data };
        }
        return card;
      });
      return updated;
    });
  }, [translate]);
  
  useEffect(() => {
    if (appointments) {
      updateCardData("appointment_card_title", { type: DICT_TYPE, data: appointments });
    }
    if (cartItems) {
      updateCardData("shopping_card_title", { type: LIST_TYPE, data: cartItems });
    }
    if (orders) {
      updateCardData("orders_card_title", { type: DICT_TYPE, data: orders });
    }
  }, [appointments, cartItems, orders, updateCardData]);

  const getUserAppointments = () => {
    api?.getAppointmentsByUserId(
      username,
      token,
      updateAppointments,
      handleError
    );
  };

  const handleUserOrders = () => {
    const orderNum = {};
    statuses.forEach(status => {
      api?.getOrdersByUserId(
        username,
        status,
        token,
        (orders) => {
          setDataLoaderCount(prev => prev + 1);
          orderNum[status] = [orders?.length];
          setOrders(prev => {
            return {
              ...prev,
              ...orderNum
        }});
        },
        handleError
      );
    });
  };

  const getUserData = () => {
    getUserAppointments();
    handleUserOrders();
  };

  const getTenantBookedAppointments = () => {
    api?.getBookedAppointments(
      getCurrentDate(),
      getCurrentMonthLastDate(),
      token,
      updateAppointments,
      handleError
    );
  };

  const getTenantOrders = () => {
    const orderNum = {};

    statuses.forEach(status => {
      api?.getOrdersByStatus(
        status,
        token,
        (orders) => {
          setDataLoaderCount(prev => prev + 1);
          orderNum[status] = [orders?.length];
          setOrders(prev => {
            return {
              ...prev,
              ...orderNum
        }});
        },
        handleError
      );
    });
  };

  const getTenantData = () => {
    getTenantBookedAppointments();
    getTenantOrders();
  };

  useFocusEffect(
    useCallback(() => {
      showLoader();
      if(isUser()) {
        getUserData();
      } else if(isTenant()) {
        getTenantData();
      }
    },[])
  );

  useEffect(() => {
    if(dataLoaderCount === API_CALLS) {
      setDataLoaderCount(0);
      hideLoader();
    }
  },[dataLoaderCount]);

  const updateAppointments = (appointments) => {
    setDataLoaderCount(prev => prev + 1);
    setAppointments(appointments?.value)
  };

  const handleGoToStore = () => navigation.navigate("Shop");
  const handleGoToCart = () => navigation.navigate("Shopping Cart");
  const handleGoToOrders = () => navigation.navigate("Orders");
  const handleGoToBooking = () => navigation.navigate("Booking");
  const handleGoToAppointmentsSettings = () => navigation.navigate("Booking Settings");
  
  useEffect(() => {
    if(toBeRemoved && toBeRemoved !== '') {
      api.removeAppointment(toBeRemoved.id, username, token, handleSuccessfulAppointmentRemove, handleError);
    }
  },[toBeRemoved]);

  const handleSuccessfulAppointmentRemove = () => {
    hideLoader();
    Toast.show({
      type: SUCCESS,
      text1: translate["action_success"],
    });
    setAppointments(prev => {
      const updated = {};
      Object.keys(prev).forEach(date => {
        updated[date] = prev[date].filter(appointment => appointment.id !== toBeRemoved.id);
      });
      return updated;
    });
    setToBeRemoved('');
  };

  const handleError = (error) => {
    hideLoader();
    setToBeRemoved('');
    Toast.show({
      type: ERROR,
      text1: translate["something_went_wrong"],
      text2: error,
    });
  }

  return (
    <View style={ globalStyles.container }>
      <ScrollView>
        <Cards cards={ cardsData }/>
      </ScrollView>
    </View>
  );
};

export default HomeScreen;