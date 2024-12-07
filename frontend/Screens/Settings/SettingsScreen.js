import React, { useContext, useEffect, useState, useCallback } from "react";
import { ScrollView, View } from "react-native";
import GlobalStyles from "@Utilities/Styles";
import { translate } from "@Utilities/translate";
import { UserContext } from "@Contexts/UserContext";
import PressableButton from "@Components/PressableButton/PressableButton";
import Cards from "@Components/Cards/Cards";
import { api } from '@Services/API';
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import UserInfo from "./UserInfo";
import TenantInfo from "./TenantInfo";
import ThemeCard from "./ThemeCard";
import { ERROR, SUCCESS, LIST_TYPE, LABEL, TEXT } from "@Utilities/Constants";

const VALUE = 'value';

const SettingsScreen = () => {
    const { username, isUser, isTenant, logOutFunction, token } = useContext(UserContext);
    const [ cardsData, setCardsData ] = useState([]);
    const [ userInfo, setUserInfo ] = useState([]);
    const [ tenantInfo, setTenantInfo ] = useState([]);
    const { showLoader, hideLoader } = useLoader();
    const globalStyles = GlobalStyles();

    useEffect(() => {
        initCardsData();
        if(isUser()) {
          initUserCardsData();
        }
    },[]);

    const initCardsData = () => {
        api.getTenantById(
            token,
            handleTenantResponse,
            handleError
        );
        setCardsData([
            {
                title: translate["tenant_card_title"],
                icon: 'store',
                list: tenantInfo,
                renderItem: (tenant) => {
                    return(
                        <TenantInfo
                            tenant={ tenant }
                            isTenant={ isTenant }
                            handleError={ handleError }
                            handleSuccessEdit={ handleSuccessEdit }
                            mapFieldsToObject={ mapFieldsToObject }
                            token={ token }
                            showLoader={ showLoader }
                        />
                    )
                }
            }
        ]);
    };

    const initUserCardsData = () => {
        api.getUserById(
            username,
            token,
            handleUserResponse,
            handleError
        );
        setCardsData(prev => {
            return(
              [
                ...prev,
                {
                    title: translate["user_card_title"],
                    icon: 'user',
                    list: userInfo,
                    renderItem: (user) => {
                        return(
                            <UserInfo 
                                user={ user }
                                handleError={ handleError }
                                handleSuccessEdit={ handleSuccessEdit }
                                mapFieldsToObject={ mapFieldsToObject }
                                token={ token }
                                showLoader={ showLoader }
                            />
                        )
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
      
    // Update cards data when user info is fetched
    useEffect(() => {
      if (userInfo) {
        updateCardData("user_card_title", { type: LIST_TYPE, data: userInfo });
      }
      if (tenantInfo) {
          updateCardData("tenant_card_title", { type: LIST_TYPE, data: tenantInfo });
        }
    }, [userInfo, tenantInfo, updateCardData]);

    // Map fields to object
    const mapFieldsToObject = (arr) => {
      const obj = {};
      arr.forEach(item => {
        if (item.hasOwnProperty(TEXT)) {
            const key = item.apiKey;
          if(item.hasOwnProperty(LABEL)) {
              obj[key] = item.text;
          } else {
              obj[key] = item.text.split(': ')[1];
          }
        }
        if (item.hasOwnProperty(VALUE)) {
          obj[item.apiKey] = item.value;
        }
      });
      return obj;
    }

    const handleUserResponse = (response) => {
        setUserInfo([response]);
    };

    const handleTenantResponse = (response) => {
        setTenantInfo([response]);
    };

    const handleSuccessEdit = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
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
        <View style={ globalStyles.container }>
            <ScrollView>
                <Cards cards={ cardsData }/>
                <ThemeCard/>
            </ScrollView>
            <PressableButton onPressFunction={ logOutFunction } icon="right">
                { translate["logout_button_title"] }
            </PressableButton>
        </View>
    );
};

export default SettingsScreen;