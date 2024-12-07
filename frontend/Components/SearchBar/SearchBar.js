import React from "react";
import { SearchBar as SB } from "react-native-elements";
import SearchBarStyles from "./SearchBarStyles";
import Icon from "@Components/Icon/Icon";

const SearchBar = ({ value, onChangeText, placeholder, inputStyle }) => {
    const styles = SearchBarStyles();

    return(
        <SB 
            containerStyle={ styles.containerStyle }
            inputContainerStyle={ styles.inputContainerStyle }
            inputStyle={ inputStyle }
            placeholderTextColor={ styles.inputContainerStyle.color }
            placeholder={ placeholder }
            onChangeText={ onChangeText }
            value={ value }
            searchIcon={ <Icon style={ styles.searchIcon } title="search"/> }
            clearIcon={ false }
            round
        />
    );
};

export default SearchBar;