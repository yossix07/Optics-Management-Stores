import React, { useState, useEffect, useContext } from "react";
import { useRoute } from "@react-navigation/native";
import { StyleSheet, ScrollView, Image, View, Text } from "react-native";
import PressableButton from "@Components/PressableButton/PressableButton";
import Input from "@Components/Input/Input";
import { api } from "@Services/API";
import { translate } from "@Utilities/translate";
import GlobalStyles from "@Utilities/Styles";
import { UserContext } from "@Contexts/UserContext";
import * as ImagePicker from 'expo-image-picker';
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import Icon from "@Components/Icon/Icon";
import ItemFormScreenStyles from "./ItemFormScreenStyles";
import MyText from "@Components/MyText/MyText";
import { useModal } from "@Hooks/UseModal";
import { ADD_MODE, EDIT_MODE, ERROR, SUCCESS } from "@Utilities/Constants";

IMAGE_SIZE = 200;

const ItemFormScreen = ({ navigation }) => {
    const { token } = useContext(UserContext);
    const route = useRoute();
    const [formData, setFormData] = useState({
        name: '',
        description: '',
        price: '',
        stock: '',
        image: null
    });
    const { showLoader, hideLoader } = useLoader();
    const { showModal, hideModal } = useModal();
    const [image, setImage] = useState(null);
    const [uploadImage, setUploadImage] = useState(false);

    const mode = route?.params?.mode;
    const item = route?.params?.item;

    useEffect(() => {
        if(mode && mode === EDIT_MODE && item && item.id){
            api.getProductById(item.id, token, parseItemToForm, handleError);
        }
    },[]);

    const parseItemToForm = (item) => {
        Object.keys(item)?.map(key=> handleInputChange(key, item[key]));
        if(item.image) {
            setImage(`data:image/png;base64,${item.image}`);
        }
    }

    const handleError = (error) => {
        hideLoader();
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error,
          });
    }
    
    const handleInputChange = (name, value) => {
      setFormData((prevData) => ({ ...prevData, [name]: value }));
    };

    const activateModal = () => {
        showModal(
            translate["confirm_changes_message"],
            handleApprove,
            hideModal
        );
    };

    const navigateBack = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
        navigation.goBack();
    }

    const handleApprove = () => {
        showLoader();
        if(mode === EDIT_MODE) {
            api.editProduct(formData, token, navigateBack, handleError);
        } else if(mode === ADD_MODE) {
            api.addProduct(formData, token, navigateBack, handleError);
        }
    }

    const pickImage = async () => {
        let result = await ImagePicker.launchImageLibraryAsync({
        mediaTypes: ImagePicker.MediaTypeOptions.All,
        allowsEditing: true,
        base64: true,
        aspect: [4, 3],
        quality: 1,
        });
        setImage(result.assets[0].uri);
        handleInputChange("image", result.assets[0].base64);
    };

    const openCamera = async () => {
        // Ask the user for the permission to access the camera
        const permissionResult = await ImagePicker.requestCameraPermissionsAsync();
        
        if (permissionResult.granted === false) {
          alert(translate["you_refused_to_allow_this_app_to_access_your_camera"]);
          return;
        }
        
        const result = await ImagePicker.launchCameraAsync({
            mediaTypes: ImagePicker.MediaTypeOptions.All,
            allowsEditing: true,
            base64: true,
            aspect: [4, 3],
            quality: 1,
        });
        if(result) {
            setImage(result.assets[0].uri);
            handleInputChange("image", result.assets[0].base64);
        }
    };
    
    const globalStyles = GlobalStyles();
    const styles = ItemFormScreenStyles();

    return(
        <ScrollView style={ StyleSheet.compose(globalStyles.container, styles.container) } contentContainerStyle={ styles.center }>
            <MyText style={ styles.title }>{ translate[`${mode}_product_title`] }</MyText>
            <Input 
                placeholder={ translate['name_placeholder'] }
                onChangeText={(text) => handleInputChange('name', text)}
                value={ formData.name }
            />
            <Input 
                placeholder={ translate['description_placeholder'] }
                onChangeText={(text) => handleInputChange('description', text)}
                value={ formData.description }
            />
            <Input 
                placeholder={ translate['price_placeholder'] }
                onChangeText={(text) => handleInputChange('price', text)}
                value={ formData.price?.toString() }
            />
            <Input 
                placeholder={ translate['stock_placeholder'] }
                onChangeText={ (text) => handleInputChange('stock', text) }
                value={ formData.stock?.toString() }
            />
            {
                !uploadImage && <PressableButton onPressFunction={ () => setUploadImage(true) }>
                    { translate["upload_image"] }
                </PressableButton>
            }
            {
                uploadImage && 
                <View style={ styles.imageButtons }>
                    <PressableButton onPressFunction={ pickImage } icon="image">
                        { translate["gallery"] }
                    </PressableButton>
                    <PressableButton onPressFunction={ openCamera } icon="camera" >
                        { translate["camera"] }
                    </PressableButton>
                </View>
            }

            {
                image &&
                <Image source={{ uri: image }} style={{ width: IMAGE_SIZE, height: IMAGE_SIZE }} />
            }
            <PressableButton onPressFunction={ activateModal }>
                <Text style={ styles.submitButtonText }>
                    { translate["submit_button"] } 
                </Text>
            </PressableButton>
        </ScrollView>
    );
};

export default ItemFormScreen;