function formatJapaneseDate(dateInput) {
    const date = dateInput instanceof Date ? dateInput : new Date(dateInput);

    if (isNaN(date)) return '';

    const year = date.getFullYear();
    const month = date.getMonth() + 1; // 1-12
    const day = date.getDate();

    return `${year}年${month}月${day}日`;
}

function formatJapaneseAge(age) {
    if (age == null || isNaN(age)) return '';
    return `${age} 歳`;
}
