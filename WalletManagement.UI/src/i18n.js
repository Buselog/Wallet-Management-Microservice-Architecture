import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

i18n
    .use(LanguageDetector)
    .use(initReactI18next)
    .init({
        resources: {
            en: {
                translation: {
                    "login_title": "Login",
                    "login_subtitle": "Connect to your digital assistant and manage your wallet.",
                    "email_label": "EMAIL",
                    "email_placeholder": "example@gmail.com",
                    "email_required": "Email cannot be empty.",
                    "email_invalid": "Please enter a valid email.",
                    "password_label": "PASSWORD",
                    "password_required": "Password cannot be empty.",
                    "login_button": "Login",
                    "no_account": "Don't have an account?",
                    "register_link": "Sign Up",
                    "hero_title": "Manage Your Financial Future",
                    "hero_subtitle": "Track all your assets from a single secure point, increase your efficiency."
                }
            },
            tr: {
                translation: {
                    "login_title": "Giriş Yap",
                    "login_subtitle": "Dijital asistanına bağlan ve cüzdanını yönet.",
                    "email_label": "E-POSTA",
                    "email_placeholder": "example@gmail.com",
                    "email_required": "E-posta alanı boş bırakılamaz.",
                    "email_invalid": "Geçerli bir e-posta giriniz.",
                    "password_label": "ŞİFRE",
                    "password_required": "Şifre alanı boş bırakılamaz.",
                    "login_button": "Giriş Yap",
                    "no_account": "Henüz hesabın yok mu?",
                    "register_link": "Kaydol",
                    "hero_title": "Finansal Geleceğini Yönet",
                    "hero_subtitle": "Tüm varlıklarını tek bir güvenli noktadan takip et, verimliliğini artır."
                }
            }
        },
        fallbackLng: 'tr',
        interpolation: {
            escapeValue: false
        }
    });

export default i18n;