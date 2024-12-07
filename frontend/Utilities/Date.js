// return current year, month and day
export function getCurrentDate() {
    const today = new Date();
    const year = today.getFullYear();
    const month = parseInt(today.getMonth() + 1);
    const day = parseInt(today.getDate());
    return {
        year: year,
        month: month,
        day: day
    }
}

// return current month's last date
export function getCurrentMonthLastDate() {
    const today = new Date();
    const month = parseInt(today.getMonth() + 1);
    return getMonthLastDate(month);
}

// return received month's last date
export function getMonthLastDate(month) {
    const year = new Date().getFullYear();
    const day = new Date(year, month, 0).getDate();
    return {
        year: parseInt(year),
        month: parseInt(month),
        day: parseInt(day)
    }
}

// return the date of the first day of the month of x months ago
export function getMonthFirstDateXmonthsAgo(x) {
    var currentDate = new Date();

    var targetMonth = currentDate.getMonth() + 1 - x;
    var targetYear = currentDate.getFullYear();

    if (targetMonth < 0) {
    targetMonth += 12;
    targetYear--;
    }

    return {
        year: targetYear,
        month: targetMonth,
        day: 1
    }
}

// return the date of the first day of the month of x months ahead
export function getMonthFirstDateXmonthsAhead(x) {
    var currentDate = new Date();

    // Calculate the target month and year
    var targetMonth = currentDate.getMonth() + 1 + x;
    var targetYear = currentDate.getFullYear();

    if (targetMonth > 11) {
    targetMonth -= 12;
    targetYear++;
    }

    return {
        year: targetYear,
        month: targetMonth,
        day: 1
    }
}

// returns true if the received string is a valid date
export function isDateString(dateString) {
    const date = new Date(dateString);
    return date instanceof Date && !isNaN(date);
}

// returns true if the received string is the first day of the next month
export function isFirstDayOfNextMonth(dateString) {
    const date = new Date(dateString);
    const currentDate = new Date();
    
    if (date.getMonth() === currentDate.getMonth() + 1 && date.getFullYear() === currentDate.getFullYear()) {
      return true;
    }

    if (date.getMonth() === 0 && currentDate.getMonth() === 11 && date.getFullYear() === currentDate.getFullYear() + 1) {
      return true;
    }

    return false;
  }
  