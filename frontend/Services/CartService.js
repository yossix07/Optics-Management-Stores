import AsyncStorage from '@react-native-async-storage/async-storage';
import { tenantID } from '../env';

const storageName = (username) => {
  return `cart-${tenantID}-${username}`;
};

async function setItems(items, username) {
  try {
    await AsyncStorage.setItem(storageName(username), JSON.stringify(items)); 
    return true;
  } catch (error) {
    return false;
  }
}

async function addItem(item, username) {
  try {
    getItems(username).then(async (items) =>{
      await AsyncStorage.setItem(storageName(username), JSON.stringify([...items, item]));
      return true;
    });
  } catch (error) {
    return false;
  }
}

async function getItems(username) {
  try {
    const items = await AsyncStorage.getItem(storageName(username));
    return JSON.parse(items);
  } catch (error) {
    return [];
  }
}

async function removeItem(toBeRemovedItem) {
  getItems(username).then((items) => {
    const newItems = items.filter(item => item.id !== toBeRemovedItem.id);
    setItems(newItems);
  }); 
}

async function removeAllItems(username) {
  try {
    await AsyncStorage.setItem(storageName(username), []);
    return true;
  } catch (error) {
    return false;
  }
}

export const cartAPI = {
  setItems: setItems,
  addItem: addItem,
  getItems: getItems,
  removeItem: removeItem,
  removeAllItems: removeAllItems
}