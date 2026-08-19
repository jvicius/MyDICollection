# 🌌 Disney Infinity Collection Tracker

A modern, fast, and 100% offline-first application built with **.NET MAUI** to easily manage and track your Disney Infinity collection.

Whether you're hunting for figures at a retro game store or organizing your display at home, MyDICollection lets you know exactly what you own, what you're missing, and how many duplicates you have.

## Preview

<div align="center">
  <video src="https://github.com/user-attachments/assets/6f03c28e-680a-4c47-bf78-740a3571da07" width="300" controls="controls"></video>
</div>

## ✨ Features

* **📱 Android NFC Integration:** On supported Android devices, scan compatible Disney Infinity figures, Power Discs, and Play Set pieces directly with NFC. Identified items can be automatically added to or removed from your collection with a simple scan.
* **🏆 Achievement System:** Unlock special badges as you complete franchises, editions, figures, Power Discs, and other collection goals.
* **💿 New Power Disc Achievements:** New franchise-based achievements have been added specifically for Power Disc collectors.
* **🗃️ Expanded & Updated Database:** Major database update with improved figure, Power Disc, and Play Set information, including model numbers and additional catalog data.
* **💾 Backup & Restore:** Export a backup of your collection and restore it later—perfect when changing devices or keeping a safe copy of your collection progress.
* **🐛 In-App Issue Reporting:** Found a bug, missing item, or incorrect information? Open a pre-filled GitHub issue directly from the app without sharing API keys or credentials.
* **📦 Precise Collection Tracking:** Keep track of your entire collection with separate and organized lists for Figures and Power Discs.
* **📶 Offline-First:** Your collection database and images are stored locally, so the core collection-tracking experience does not require an internet connection.
* **🌗 Native Theme Support:** A polished interface that adapts seamlessly to your device's Light and Dark mode.
* **🌐 Bilingual (On-Demand):** Fully translated into English and Spanish, with language switching available directly from the app settings.
* **🔍 Accessible & Lightning-Fast Filters:** Quickly sort and filter your collection by franchise, item type, and other available categories.

## 🎉 What's New in v1.0.2

Version **1.0.2** is a major step forward for MyDICollection:

* Full **Android NFC identification workflow** for supported Disney Infinity collectibles.
* Add compatible collectibles to your collection automatically by scanning them with NFC.
* Remove owned collectibles from your collection by scanning them again through the appropriate app action.
* New **Power Disc achievements**, including franchise-based collection goals.
* Major **figure, Power Disc, and Play Set database expansion and cleanup**.
* New **collection backup and restore** functionality.
* New **issue reporting** flow that opens GitHub with the report information ready to submit.
* General catalog, data, and usability improvements.

> **Note:** NFC functionality is read-only and intended exclusively for identifying physical collectibles for personal collection management. MyDICollection does not clone, emulate, modify, or write Disney Infinity NFC tags.

## 🚀 Roadmap / Upcoming Features

- [x] **NFC Identification (Android):** Identify compatible physical collectibles using NFC and manage collection ownership from supported Android devices.
- [x] **Power Disc Overhaul:** Expanded Power Disc catalog and dedicated achievements.
- [x] **Collection Backup & Restore:** Export and restore collection progress when moving to another device or keeping a backup.
- [x] **In-App Issue Reporting:** Quickly prepare and submit bug reports or data corrections through GitHub.
- [ ] **iOS Physical Testing:** Test MyDICollection on physical iPhone/iPad hardware and prepare for a stable iOS release.
- [ ] **NFC Identification (iOS):** Research, implement, and test read-only collectible identification on supported Apple devices.
- [ ] **NFC Identification (Windows):** Identify compatible collectibles on Windows using supported NFC hardware / the Disney Infinity Base where technically feasible.

## 📥 Installation

Go to the [Releases](../../releases) tab to download the latest version for your device:

* **Android:** Download the `.apk` file and install it directly on your phone.
* **Windows:** Download the `.exe` release for your PC.

> iOS builds are not publicly distributed yet. Physical-device testing and an Apple Developer membership are still required before an iOS release can be prepared.

## ☕ Support the Project

MyDICollection is a personal, open-source project developed in my free time.

If you enjoy the app and would like to help keep development moving, contributions through Ko-fi help fund project expenses and future platform support.

### Current goals

* 🍏 **Apple Developer membership** for iOS testing and distribution.
* 📱 **Physical iOS testing** and development of NFC identification for supported Apple devices.
* 🖥️ **Windows NFC research and development**, including testing with compatible hardware.
* 🛠️ Continued database maintenance, testing, and development.

So far, the project has received **$10 USD** in community support. Thank you to everyone who has contributed!

[![Ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/josevelarde)

## 🤝 Contributing & Reporting Issues

MyDICollection is community-driven, and feedback is welcome.

If you find an incorrect model number, missing collectible, NFC identification problem, bug, or other issue, you can use the reporting option built into the app or open an issue directly through this repository.

When reporting an NFC identification problem, please include the collectible name, app version, device/platform, and any non-sensitive diagnostic information that may help reproduce the issue.

## ⚖️ Legal & Trademark Notice

MyDICollection is an independent, community-developed collection-management
project. It is not affiliated with, endorsed by, sponsored by, or associated
with The Walt Disney Company or its affiliates.

Disney Infinity, Disney, Pixar, Marvel, Star Wars, and related names,
characters, logos, trademarks, and other intellectual property are the
property of their respective owners.

References to these properties are used solely to identify compatible
collectible items and to provide collection-management functionality.

NFC functionality in MyDICollection is intended for read-only identification
of physical collectible items for personal collection management and
interoperability purposes. MyDICollection does not provide functionality
for cloning, emulating, modifying, or writing Disney Infinity NFC tags.

## 📚 Third-Party Notices

Development of the NFC identification feature was informed in part by
publicly available interoperability research from the open-source Proxmark3
project maintained by RfidResearchGroup and its contributors.

See [`THIRD_PARTY_NOTICES.md`](THIRD_PARTY_NOTICES.md) for attribution,
provenance, and third-party licensing information.
