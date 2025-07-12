import sys
import subprocess
from datetime import datetime, timedelta
from PyQt6.QtWidgets import (QApplication, QMainWindow, QWidget, QVBoxLayout, 
                             QHBoxLayout, QLabel, QSpinBox, QPushButton, 
                             QTextEdit, QGroupBox, QRadioButton, QMessageBox,
                             QGridLayout)
from PyQt6.QtCore import QTimer, Qt, pyqtSignal
from PyQt6.QtGui import QIcon, QFont, QPixmap, QPainter, QPen

class ShutdownTimer(QMainWindow):
    def __init__(self):
        super().__init__()
        self.timer = QTimer()
        self.timer.timeout.connect(self.update_countdown)
        self.shutdown_time = None
        self.countdown_seconds = 0
        self.is_running = False
        
        self.init_ui()
    
    def create_icon(self):
        """创建应用程序图标 - 基于SVG闹钟设计"""
        try:
            # 创建64x64的图标以获得更好的清晰度
            pixmap = QPixmap(64, 64)
            pixmap.fill(Qt.GlobalColor.transparent)
            
            painter = QPainter(pixmap)
            painter.setRenderHint(QPainter.RenderHint.Antialiasing)
            
            # 定义颜色
            from PyQt6.QtGui import QColor
            red_color = QColor(229, 115, 115)  # #E57373
            blue_gray = QColor(144, 164, 174)  # #90A4AE
            white_color = QColor(255, 255, 255)  # #FFFFFF
            
            # 缩放因子：从SVG的100x100缩放到64x64
            scale = 0.64
            
            # 偏移量，使图标居中
            offset_x, offset_y = 0, 1.3
            
            # 绘制闹钟的脚
            pen = QPen(blue_gray, 3 * scale)
            pen.setCapStyle(Qt.PenCapStyle.RoundCap)
            painter.setPen(pen)
            painter.drawLine(int((28 * scale) + offset_x), int((82 * scale) + offset_y), 
                           int((20 * scale) + offset_x), int((92 * scale) + offset_y))
            painter.drawLine(int((72 * scale) + offset_x), int((82 * scale) + offset_y), 
                           int((80 * scale) + offset_x), int((92 * scale) + offset_y))
            
            # 绘制闹钟的铃铛
            painter.setPen(QPen(blue_gray, 1))
            painter.setBrush(blue_gray)
            painter.drawEllipse(int((28 * scale - 10 * scale) + offset_x), int((22 * scale - 10 * scale) + offset_y), 
                              int(20 * scale), int(20 * scale))
            painter.drawEllipse(int((72 * scale - 10 * scale) + offset_x), int((22 * scale - 10 * scale) + offset_y), 
                              int(20 * scale), int(20 * scale))
            
            # 绘制顶部的按钮/撞锤
            pen = QPen(blue_gray, 1.6 * scale)
            pen.setCapStyle(Qt.PenCapStyle.RoundCap)
            painter.setPen(pen)
            painter.drawLine(int((50 * scale) + offset_x), int((23 * scale) + offset_y), 
                           int((50 * scale) + offset_x), int((18 * scale) + offset_y))
            
            # 绘制按钮矩形
            painter.setBrush(blue_gray)
            painter.setPen(QPen(blue_gray, 1))
            painter.drawRoundedRect(int((44 * scale) + offset_x), int((15 * scale) + offset_y), 
                                  int(12 * scale), int(5 * scale), 1.6 * scale, 1.6 * scale)
            
            # 绘制闹钟主体 - 外圆
            pen = QPen(red_color, 5 * scale)
            painter.setPen(pen)
            painter.setBrush(Qt.BrushStyle.NoBrush)
            painter.drawEllipse(int((50 * scale - 32 * scale) + offset_x), int((55 * scale - 32 * scale) + offset_y), 
                              int(64 * scale), int(64 * scale))
            
            # 绘制闹钟主体 - 内圆（白色填充）
            painter.setBrush(white_color)
            painter.setPen(QPen(white_color, 1))
            painter.drawEllipse(int((50 * scale - 28 * scale) + offset_x), int((55 * scale - 28 * scale) + offset_y), 
                              int(56 * scale), int(56 * scale))
            
            # 绘制指针
            center_x = int((50 * scale) + offset_x)
            center_y = int((55 * scale) + offset_y)
            
            # 时针 (旋转55度，长度15)
            painter.save()
            painter.translate(center_x, center_y)
            painter.rotate(55)
            pen = QPen(blue_gray, 3.3 * scale)
            pen.setCapStyle(Qt.PenCapStyle.RoundCap)
            painter.setPen(pen)
            painter.drawLine(0, 0, 0, int(-15 * scale))
            painter.restore()
            
            # 分针 (旋转-60度，长度23)
            painter.save()
            painter.translate(center_x, center_y)
            painter.rotate(-60)
            pen = QPen(blue_gray, 3.3 * scale)
            pen.setCapStyle(Qt.PenCapStyle.RoundCap)
            painter.setPen(pen)
            painter.drawLine(0, 0, 0, int(-23 * scale))
            painter.restore()
            
            # 秒针 (指向6点钟方向，长度20)
            pen = QPen(red_color, 1.3 * scale)
            pen.setCapStyle(Qt.PenCapStyle.RoundCap)
            painter.setPen(pen)
            painter.drawLine(center_x, center_y, center_x, center_y + int(20 * scale))
            
            # 中心点
            painter.setBrush(blue_gray)
            painter.setPen(QPen(blue_gray, 1))
            painter.drawEllipse(center_x - int(1.9 * scale), center_y - int(1.9 * scale), 
                              int(3.8 * scale), int(3.8 * scale))
            
            painter.end()
            return QIcon(pixmap)
        except Exception as e:
            print(f"创建图标时出错: {e}")
            # 如果出错，返回默认图标
            return QIcon()
        
    def init_ui(self):
        self.setWindowTitle("⏰ 定时关机软件")
        self.setFixedSize(520, 680)
        
        # 设置窗口图标
        self.setWindowIcon(self.create_icon())
        
        # 设置窗口背景
        self.setStyleSheet("QMainWindow { background-color: #f8f9fa; }")
        
        # 创建中央部件
        central_widget = QWidget()
        self.setCentralWidget(central_widget)
        
        # 主布局
        main_layout = QVBoxLayout(central_widget)
        
        # 标题
        title_label = QLabel("⏰ 定时关机软件")
        title_label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        title_label.setFont(QFont("微软雅黑", 20, QFont.Weight.Bold))
        title_label.setStyleSheet("""
            QLabel {
                color: #2c3e50;
                background: qlineargradient(x1: 0, y1: 0, x2: 1, y2: 0,
                    stop: 0 #ecf0f1, stop: 1 #bdc3c7);
                padding: 15px;
                border-radius: 8px;
                margin: 10px;
                border: 2px solid #34495e;
            }
        """)
        main_layout.addWidget(title_label)
        
        # 时间设置组
        time_group = QGroupBox("⏱️ 设置关机时间")
        time_layout = QVBoxLayout(time_group)
        
        # 创建时间输入的水平布局
        time_input_layout = QHBoxLayout()
        time_input_layout.setSpacing(20)  # 增加间距
        
        # 小时设置
        hour_container = QHBoxLayout()
        hour_container.setSpacing(8)  # 设置数字框和标签的间距
        self.hour_spinbox = QSpinBox()
        self.hour_spinbox.setRange(0, 23)
        self.hour_spinbox.setValue(0)
        self.hour_spinbox.setMinimumWidth(60)
        self.hour_spinbox.setMaximumWidth(60)
        hour_container.addWidget(self.hour_spinbox)
        hour_label = QLabel("小时")
        hour_label.setAlignment(Qt.AlignmentFlag.AlignLeft | Qt.AlignmentFlag.AlignVCenter)
        hour_container.addWidget(hour_label)
        time_input_layout.addLayout(hour_container)
        
        # 分钟设置
        minute_container = QHBoxLayout()
        minute_container.setSpacing(8)  # 设置数字框和标签的间距
        self.minute_spinbox = QSpinBox()
        self.minute_spinbox.setRange(0, 59)
        self.minute_spinbox.setValue(0)
        self.minute_spinbox.setMinimumWidth(60)
        self.minute_spinbox.setMaximumWidth(60)
        minute_container.addWidget(self.minute_spinbox)
        minute_label = QLabel("分钟")
        minute_label.setAlignment(Qt.AlignmentFlag.AlignLeft | Qt.AlignmentFlag.AlignVCenter)
        minute_container.addWidget(minute_label)
        time_input_layout.addLayout(minute_container)
        
        # 秒钟设置
        second_container = QHBoxLayout()
        second_container.setSpacing(8)  # 设置数字框和标签的间距
        self.second_spinbox = QSpinBox()
        self.second_spinbox.setRange(0, 59)
        self.second_spinbox.setValue(5)
        self.second_spinbox.setMinimumWidth(60)
        self.second_spinbox.setMaximumWidth(60)
        second_container.addWidget(self.second_spinbox)
        second_label = QLabel("秒")
        second_label.setAlignment(Qt.AlignmentFlag.AlignLeft | Qt.AlignmentFlag.AlignVCenter)
        second_container.addWidget(second_label)
        time_input_layout.addLayout(second_container)
        
        # 添加弹性空间
        time_input_layout.addStretch()
        
        time_layout.addLayout(time_input_layout)
        
        # 关机选项
        option_group = QGroupBox("🔌 关机选项")
        option_layout = QVBoxLayout(option_group)
        option_layout.setSpacing(8)  # 增加选项之间的间距
        option_layout.setContentsMargins(15, 10, 15, 15)  # 增加内边距
        
        self.hibernate_radio = QRadioButton("😴 休眠")
        self.hibernate_radio.setChecked(True)
        option_layout.addWidget(self.hibernate_radio)
        
        self.force_hibernate_radio = QRadioButton("💤 强制休眠（不保存数据）")
        option_layout.addWidget(self.force_hibernate_radio)
        
        self.shutdown_radio = QRadioButton("🔌 关机")
        option_layout.addWidget(self.shutdown_radio)
        
        self.restart_radio = QRadioButton("🔄 重启")
        option_layout.addWidget(self.restart_radio)
        
        self.force_checkbox = QRadioButton("⚠️ 强制关机（不保存数据）")
        option_layout.addWidget(self.force_checkbox)
        
        # 控制按钮
        button_layout = QHBoxLayout()
        
        self.start_button = QPushButton("开始定时")
        self.start_button.clicked.connect(self.start_timer)
        self.start_button.setStyleSheet("QPushButton { background-color: #4CAF50; color: white; font-weight: bold; padding: 10px; border-radius: 5px; }")
        button_layout.addWidget(self.start_button)
        
        self.stop_button = QPushButton("取消定时")
        self.stop_button.clicked.connect(self.stop_timer)
        self.stop_button.setEnabled(False)
        self.stop_button.setStyleSheet("QPushButton { background-color: #f44336; color: white; font-weight: bold; padding: 10px; border-radius: 5px; }")
        button_layout.addWidget(self.stop_button)
        
        # 状态显示
        self.status_label = QLabel("状态：等待设置")
        self.status_label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.status_label.setFont(QFont("微软雅黑", 12))
        self.status_label.setStyleSheet("QLabel { color: #2196F3; font-weight: bold; padding: 5px; }")
        
        # 倒计时显示
        self.countdown_label = QLabel("倒计时：--:--:--")
        self.countdown_label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.countdown_label.setFont(QFont("微软雅黑", 14, QFont.Weight.Bold))
        self.countdown_label.setStyleSheet("QLabel { color: #FF5722; background-color: #FFF3E0; padding: 10px; border-radius: 5px; border: 2px solid #FF5722; }")
        
        # 日志区域
        log_group = QGroupBox("📋 操作日志")
        log_layout = QVBoxLayout(log_group)
        
        self.log_text = QTextEdit()
        self.log_text.setMaximumHeight(150)
        self.log_text.setReadOnly(True)
        self.log_text.setStyleSheet("QTextEdit { background-color: #f5f5f5; border: 1px solid #ccc; }")
        log_layout.addWidget(self.log_text)
        
        # 添加所有组件到主布局
        main_layout.addWidget(time_group)
        main_layout.addWidget(option_group)
        main_layout.addLayout(button_layout)
        main_layout.addWidget(self.status_label)
        main_layout.addWidget(self.countdown_label)
        main_layout.addWidget(log_group)
        
        # 设置整体样式
        self.setStyleSheet("""
            QGroupBox {
                font-weight: bold;
                font-size: 14px;
                color: #2c3e50;
                border: 2px solid #3498db;
                border-radius: 8px;
                margin-top: 15px;
                padding-top: 15px;
                background-color: #ffffff;
            }
            QGroupBox::title {
                subcontrol-origin: margin;
                subcontrol-position: top center;
                padding: 5px 10px;
                background-color: #3498db;
                color: white;
                border-radius: 4px;
                font-size: 13px;
            }
            QLabel {
                font-size: 12px;
            }
            QSpinBox {
                padding: 6px 8px;
                border: 1px solid #cccccc;
                border-radius: 3px;
                font-size: 14px;
                min-height: 25px;
            }
            QSpinBox::up-button {
                width: 0px;
                height: 0px;
                border: none;
                background: none;
            }
            QSpinBox::down-button {
                width: 0px;
                height: 0px;
                border: none;
                background: none;
            }

            QRadioButton {
                spacing: 12px;
                margin: 12px 8px;
                font-size: 16px;
                font-weight: 500;
                color: #2c3e50;
                padding: 8px 12px;
                min-height: 20px;
            }
            QRadioButton::indicator {
                width: 20px;
                height: 20px;
                border-radius: 10px;
                border: 2px solid #3498db;
                background-color: #ffffff;
            }
            QRadioButton::indicator:checked {
                background-color: #3498db;
                border: 3px solid #2980b9;
            }

            QRadioButton:hover {
                background-color: #ecf0f1;
                border-radius: 5px;
            }
        """)
        
        # 添加初始日志
        self.add_log("程序启动成功")
    

    
    def start_timer(self):
        """开始定时器"""
        hours = self.hour_spinbox.value()
        minutes = self.minute_spinbox.value()
        seconds = self.second_spinbox.value()
        
        if hours == 0 and minutes == 0 and seconds == 0:
            QMessageBox.warning(self, "警告", "请设置有效的时间！")
            return
        
        self.countdown_seconds = hours * 3600 + minutes * 60 + seconds
        self.shutdown_time = datetime.now() + timedelta(seconds=self.countdown_seconds)
        
        self.is_running = True
        self.start_button.setEnabled(False)
        self.stop_button.setEnabled(True)
        
        # 立即显示初始倒计时
        hours_display = self.countdown_seconds // 3600
        minutes_display = (self.countdown_seconds % 3600) // 60
        seconds_display = self.countdown_seconds % 60
        self.countdown_label.setText(f"倒计时：{hours_display:02d}:{minutes_display:02d}:{seconds_display:02d}")
        
        # 开始倒计时
        self.timer.start(1000)  # 每秒更新一次
        
        action = self.get_selected_action()
        self.add_log(f"定时任务已开始：{hours}小时{minutes}分{seconds}秒后执行【{action}】")
        
        # 显示倒计时格式的状态
        total_seconds = self.countdown_seconds
        hours_status = total_seconds // 3600
        minutes_status = (total_seconds % 3600) // 60
        seconds_status = total_seconds % 60
        
        if hours_status > 0:
            time_str = f"{hours_status}小时{minutes_status}分{seconds_status}秒"
        elif minutes_status > 0:
            time_str = f"{minutes_status}分{seconds_status}秒"
        else:
            time_str = f"{seconds_status}秒"
            
        self.status_label.setText(f"状态：定时器运行中 - 还有{time_str}后执行{action}")
        

    
    def stop_timer(self):
        """停止定时器"""
        self.timer.stop()
        self.is_running = False
        self.start_button.setEnabled(True)
        self.stop_button.setEnabled(False)
        
        self.status_label.setText("状态：定时器已取消")
        self.countdown_label.setText("倒计时：--:--:--")
        
        self.add_log("定时任务已取消")
    
    def update_countdown(self):
        """更新倒计时"""
        if self.countdown_seconds > 0:
            self.countdown_seconds -= 1
            
            hours = self.countdown_seconds // 3600
            minutes = (self.countdown_seconds % 3600) // 60
            seconds = self.countdown_seconds % 60
            
            self.countdown_label.setText(f"倒计时：{hours:02d}:{minutes:02d}:{seconds:02d}")
            
            # 同时更新状态显示
            action = self.get_selected_action()
            if hours > 0:
                time_str = f"{hours}小时{minutes}分{seconds}秒"
            elif minutes > 0:
                time_str = f"{minutes}分{seconds}秒"
            else:
                time_str = f"{seconds}秒"
            
            self.status_label.setText(f"状态：定时器运行中 - 还有{time_str}后执行{action}")
            
        else:
            # 时间到，执行关机操作
            self.execute_shutdown()
    
    def execute_shutdown(self):
        """执行关机操作"""
        self.timer.stop()
        self.is_running = False
        
        action = self.get_selected_action()
        command = self.get_shutdown_command()
        
        try:
            self.add_log(f"倒计时结束，正在执行：{action}")
            
            # 直接执行命令，不再询问用户
            subprocess.run(command, shell=True)
            self.add_log(f"命令已执行：{' '.join(command)}")
                
        except Exception as e:
            self.add_log(f"执行失败：{str(e)}")
            QMessageBox.critical(self, "错误", f"执行失败：{str(e)}")
            self.reset_ui()
    
    def get_selected_action(self):
        """获取选中的操作"""
        if self.shutdown_radio.isChecked():
            return "关机"
        elif self.restart_radio.isChecked():
            return "重启"
        elif self.hibernate_radio.isChecked():
            return "休眠"
        elif self.force_hibernate_radio.isChecked():
            return "强制休眠"
        elif self.force_checkbox.isChecked():
            return "强制关机"
        return "休眠"
    
    def get_shutdown_command(self):
        """获取关机命令"""
        if self.shutdown_radio.isChecked():
            return ["shutdown", "/s", "/t", "0"]
        elif self.restart_radio.isChecked():
            return ["shutdown", "/r", "/t", "0"]
        elif self.hibernate_radio.isChecked():
            return ["shutdown", "/h"]
        elif self.force_hibernate_radio.isChecked():
            return ["shutdown", "/f", "/h"]
        elif self.force_checkbox.isChecked():
            return ["shutdown", "/s", "/f", "/t", "0"]
        return ["shutdown", "/h"]
    
    def reset_ui(self):
        """重置UI状态"""
        self.start_button.setEnabled(True)
        self.stop_button.setEnabled(False)
        self.status_label.setText("状态：等待设置")
        self.countdown_label.setText("倒计时：--:--:--")
    
    def add_log(self, message):
        """添加日志"""
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        self.log_text.append(f"[{timestamp}] {message}")
        # 自动滚动到底部
        scrollbar = self.log_text.verticalScrollBar()
        if scrollbar:
            scrollbar.setValue(scrollbar.maximum())
    
    def closeEvent(self, event):
        """窗口关闭事件"""
        if self.is_running:
            reply = QMessageBox.question(self, "确认", 
                                       "定时器正在运行，确定要退出吗？", 
                                       QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
            if reply == QMessageBox.StandardButton.No:
                event.ignore()
                return
        
        # 直接关闭程序
        event.accept()
    
    def quit_app(self):
        """退出应用程序"""
        if self.is_running:
            reply = QMessageBox.question(self, "确认", 
                                       "定时器正在运行，确定要退出吗？", 
                                       QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
            if reply == QMessageBox.StandardButton.No:
                return
        
        QApplication.quit()

def main():
    app = QApplication(sys.argv)
    
    window = ShutdownTimer()
    window.show()
    
    sys.exit(app.exec())

if __name__ == "__main__":
    main() 