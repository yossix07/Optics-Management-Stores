import React, { useState, useEffect, useRef, useContext, useCallback } from 'react';
import { ScrollView, StyleSheet, View } from 'react-native';
import { FAB } from 'react-native-elements';
import SearchBar from '@Components/SearchBar/SearchBar';
import ClickableItem from '@Components/ClickableItem/ClickableItem';
import { api } from '@Services/API';
import GlobalStyles from '@Utilities/Styles';
import Icon from '@Components/Icon/Icon';
import { UserContext } from "@Contexts/UserContext";
import { useFocusEffect } from "@react-navigation/native";
import { useColors } from '@Hooks/UseColors';
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import { ERROR, STORE_ITEM, ADD_MODE } from "@Utilities/Constants";

const StoreScreen = ({ navigation }) => {
    const COLORS = useColors();
    const { isUser, isTenant, token } = useContext(UserContext);
    const [ items, setItems ] = useState([]);
    const [ searchInput, setSearchInput ] = useState('');
    const { showLoader, hideLoader } = useLoader();
    var allItems = useRef([]);

    useFocusEffect(
        useCallback(() => {
            showLoader();
            api.getAllProducts(token, setItemsFunc, handleError);
    }, []));

    const setItemsFunc = (items) => {
        hideLoader();
        allItems.current.value = items;
        setItems(items);
    };

    const handleError = (error) => {
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error,
        });
    }

    useEffect(() => {
        if(searchInput === '') {
            setItems(allItems.current.value);
        } else {
            setItems(allItems.current.value.filter(item => item.name.toLowerCase().includes(searchInput.toLowerCase())));
        }
    }, [searchInput]);

    const handleSearchInput = (text) => {
        setSearchInput(text);
    };

    const handleItemClick = (item) => {
        navigation.navigate('Item',  {
            itemId: item.id,
            type: STORE_ITEM
        });
    };

    const handleCartPress = () => {
        navigation.navigate('Cart');
    };

    const handleAddItem = () => {
        navigation.navigate('Product-Form', { 
            mode: ADD_MODE,
            item: null
        });
    };

    const globalStyles = GlobalStyles();

    return(
        <View style={ globalStyles.container }>
            <ScrollView>
                <SearchBar 
                    placeholder="Search"
                    onChangeText={ handleSearchInput }
                    value={ searchInput }
                    inputStyle={{ color: COLORS.main_opposite }}
                />
                { items && items.map((item, index) => (
                        <ClickableItem
                            key={ index }
                            name={ item.name }
                            description={ item.description }
                            price={ item.price }
                            image={ item?.image }
                            clickFunc={ () => { handleItemClick(item) }}
                        />
                ))}
            </ScrollView >
            { isUser() && 
                <FAB 
                    style={ styles.button }
                    title={ <Icon title={ 'cart' }/>}
                    placement='right'
                    onPress={ handleCartPress }
                    color={ COLORS.primary }
                /> 
            }

            { isTenant() && 
                <FAB 
                    style={ styles.button }
                    title={ <Icon style={{ color: COLORS.primary_opposite }} title={ 'add' }/>}
                    placement='right'
                    onPress={ handleAddItem }
                    color={ COLORS.primary }
                /> 
            }
        </View>
    );
}

const styles = StyleSheet.create({
    button: {
        position: 'absolute',
        bottom: 0,
        right: 0,
    }
});

export default StoreScreen;