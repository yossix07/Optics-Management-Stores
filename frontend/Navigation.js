import React, { useContext } from "react";
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { createDrawerNavigator } from '@react-navigation/drawer';
import Icon from '@Components/Icon/Icon';
import { useColors } from "@Hooks/UseColors";

// screens
import LoginScreen from "@Screens/Login/LoginScreen";
import SignupScreen from "@Screens/SignUp/SignUpScreen";
import ForgotPasswordEmailScreen from "@Screens/ForgotPassword/ForgotPasswordEmailScreen";
import ResetPasswordScreen from "@Screens/ForgotPassword/ResetPasswordScreen";
import HomeScreen from "./Screens/Home/HomeScreen";
import StoreScreen from "@Screens/Store/StoreScreen";
import ItemScreen from "@Screens/Store/ItemScreen";
import CartScreen from "@Screens/Store/CartScreen";
import OrdersScreen from "@Screens/Store/OrdersScreen";
import StatsScreen from "@Screens/Stats/StatsScreen";
import ItemFormScreen from "@Screens/Store/ItemFormScreen";
import BookingScreen from "@Screens/Booking/BookingScreen";
import AppointmentsSettingsScreen from "@Screens/Booking/AppointmentsSettingsScreen";
import AddAppointmentTypeScreen from "@Screens/Booking/AddAppointmentTypeScreen";
import AddAppointmentBlockScreen from "@Screens/Booking/AddAppointmentBlockScreen";
import AddCustomAppointmentSlotScreen from "@Screens/Booking/AddCustomAppointmentSlotScreen";
import AddDayOffScreen from "@Screens/Booking/AddDayOffScreen";
import SettingsScreen from "@Screens/Settings/SettingsScreen";
import SightTestScreen from "@Screens/SightTest/SightTestScreen";
import SightTestResultScreen from "@Screens/SightTest/SightTestResultScreen";

// contexts
import { CartProvider } from "@Contexts/CartContext";
import { UserContext } from "@Contexts/UserContext";

const Stack = createNativeStackNavigator();
const Drawer = createDrawerNavigator();

const Navigation = () => {
  const { isLoggedIn, isTenant, isUser } = useContext(UserContext);
  const COLORS = useColors();

  const drawerOptions = {
    drawerStyle: {
      backgroundColor: COLORS.main,
    },
    headerStyle: {
      backgroundColor: COLORS.dark_main,
    },
    drawerActiveBackgroundColor: COLORS.primary,
    drawerActiveTintColor: COLORS.primary_opposite,
    drawerInactiveTintColor: COLORS.main_opposite,
  };

  if (isLoggedIn) {
    return (
      <CartProvider>
        <NavigationContainer>
          <Drawer.Navigator screenOptions={ drawerOptions } >
            <Drawer.Screen 
              name="Home"
              component={ HomeScreen }
              options={{
                drawerIcon: () => (
                  <Icon title="home" style={{ color: COLORS.secondary }} />
                ),
              }}
            />
            <Drawer.Screen
              name="Shop"
              component={ StoreStackNavigator }
              options={{
                drawerIcon: () => (
                  <Icon title="store" style={{ color: COLORS.secondary }} />
                ),
              }}
            />
            { 
              isUser() && 
              <Drawer.Screen
                name="Shopping Cart"
                component={ CartStackNavigator }
                options={{
                  drawerIcon: () => (
                    <Icon title="cart" style={{ color: COLORS.secondary }} />
                  ),
                }}
              /> 
            }
            <Drawer.Screen
              name="Orders"
              component={ OrdersScreen }
              options={{
                drawerIcon: () => (
                  <Icon title="truck" style={{ color: COLORS.secondary }} />
                ),
              }}
            />
            {
              isTenant() &&
              <Drawer.Screen
                name="Statistics"
                component={ StatsScreen }
                options={{
                  drawerIcon: () => (
                    <Icon title="stats" style={{ color: COLORS.secondary }} />
                  ),
                }}
              />
            }
            <Drawer.Screen
              name="Booking"
              component={ BookingScreen }
              options={{
                drawerIcon: () => (
                  <Icon title="calendar" style={{ color: COLORS.secondary }} />
                ),
              }}
            />
            { 
              isTenant() &&
              <Drawer.Screen
                name="Booking Settings"
                component={ BookingStackNavigator }
                options={{
                  drawerIcon: () => (
                    <Icon title="plusCalendar" style={{ color: COLORS.secondary }} />
                  ),
                }}
              /> 
            }
            <Drawer.Screen
              name="Sight Test"
              component={ SightTestStackNavigator }
              options={{
                drawerIcon: () => (
                  <Icon title="eye" style={{ color: COLORS.secondary }} />
                ),
              }}
            />
            <Drawer.Screen
              name="Settings"
              component={ SettingsScreen }
              options={{
                drawerIcon: () => (
                  <Icon title="gear" style={{ color: COLORS.secondary }} />
                ),
              }}
            />
          </Drawer.Navigator>
        </NavigationContainer>
      </CartProvider>
    );
  } else {
    return (
      <NavigationContainer>
        <Drawer.Navigator screenOptions={ drawerOptions } >
          <Drawer.Screen
            name="Login"
            component={ LoginScreen }
            options={{
              drawerIcon: () => (
                <Icon title="right" style={{ color: COLORS.secondary }} />
              ),
            }}
          />
          <Drawer.Screen
            name="Sign-Up"
            component={ SignupScreen }
            options={{
              drawerIcon: () => (
                <Icon title="userPlus" style={{ color: COLORS.secondary }} />
              ),
            }}
          />
          <Drawer.Screen
            name="Forgot Password"
            component={ ForgotPasswordStackNavigator }
            options={{
              drawerIcon: () => (
                <Icon title="userLock" style={{ color: COLORS.secondary }} />
              ),
            }}
          />
        </Drawer.Navigator>
      </NavigationContainer>
    );
  }
};

const BookingStackNavigator = () => {
  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      <Stack.Screen name="Appointments-Settings" component={ AppointmentsSettingsScreen } />
      <Stack.Screen name="Add-Appointment-Type" component={ AddAppointmentTypeScreen } />
      <Stack.Screen name="Add-Appointment-Block" component={ AddAppointmentBlockScreen } />
      <Stack.Screen name="Add-Custom-Appointment-Slot" component={ AddCustomAppointmentSlotScreen } />
      <Stack.Screen name="Add-Day-Off" component={ AddDayOffScreen } />
    </Stack.Navigator>
  );
};

const StoreStackNavigator = () => {
  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      <Stack.Screen name="Store" component={ StoreScreen } />
      <Stack.Screen name="Item" component={ ItemScreen } />
      <Stack.Screen name="Cart" component={ CartScreen } />
      <Stack.Screen name="Product-Form" component={ ItemFormScreen } />
    </Stack.Navigator>
  );
};

const CartStackNavigator = () => {
  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      <Stack.Screen name="Cart" component={ CartScreen } />
      <Stack.Screen name="Item" component={ ItemScreen } />
    </Stack.Navigator>
  );
};

const SightTestStackNavigator = () => {
  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      <Stack.Screen name="Sight-Test" component={ SightTestScreen } />
      <Stack.Screen name="Sight-Test-Result" component={ SightTestResultScreen } />
    </Stack.Navigator>
  );
};

const ForgotPasswordStackNavigator = () => {
  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      <Stack.Screen name="Forgot-Password-Email" component={ ForgotPasswordEmailScreen } />
      <Stack.Screen name="Reset-Password" component={ ResetPasswordScreen } />
    </Stack.Navigator>
  );
};

export default Navigation;