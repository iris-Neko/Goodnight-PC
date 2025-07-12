from PIL import Image, ImageDraw, ImageFont
import os
import math

def create_256x256_clock_icon():
    """创建256x256高质量时钟图标"""
    size = 256
    img = Image.new('RGBA', (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    center = size // 2
    
    # 绘制阴影
    shadow_offset = 3
    shadow_radius = center - 16
    shadow_color = (0, 0, 0, 60)
    draw.ellipse([center - shadow_radius + shadow_offset, center - shadow_radius + shadow_offset,
                  center + shadow_radius + shadow_offset, center + shadow_radius + shadow_offset], 
                 fill=shadow_color)
    
    # 绘制主圆圈
    main_radius = center - 8
    
    # 绘制外圆边框
    border_width = 4
    for i in range(border_width):
        r = main_radius - i
        border_color = (70, 130, 180, 255)
        draw.ellipse([center - r, center - r, center + r, center + r], 
                     outline=border_color, width=1)
    
    # 绘制主背景
    bg_color = (173, 216, 230, 255)
    draw.ellipse([center - main_radius + border_width, center - main_radius + border_width,
                  center + main_radius - border_width, center + main_radius - border_width], 
                 fill=bg_color)
    
    # 绘制内圆高光
    inner_radius = main_radius - 24
    inner_color = (220, 240, 255, 180)
    draw.ellipse([center - inner_radius, center - inner_radius,
                  center + inner_radius, center + inner_radius], 
                 fill=inner_color)
    
    # 绘制时钟刻度
    tick_radius = main_radius - 16
    tick_color = (60, 60, 60, 255)
    
    for i in range(12):
        angle = i * 30 * math.pi / 180
        
        # 12, 3, 6, 9点位置的刻度更粗更长
        if i % 3 == 0:
            inner_r = tick_radius - 24
            width = 6
        else:
            inner_r = tick_radius - 12
            width = 4
        
        x1 = center + int(inner_r * math.sin(angle))
        y1 = center - int(inner_r * math.cos(angle))
        x2 = center + int(tick_radius * math.sin(angle))
        y2 = center - int(tick_radius * math.cos(angle))
        
        draw.line([(x1, y1), (x2, y2)], fill=tick_color, width=width)
    
    # 绘制数字
    try:
        font = ImageFont.truetype("arial.ttf", 20)
    except:
        font = ImageFont.load_default()
    
    number_radius = tick_radius - 40
    text_color = (40, 40, 40, 255)
    
    for i in range(12):
        number = 12 if i == 0 else i
        angle = i * 30 * math.pi / 180
        x = center + int(number_radius * math.sin(angle))
        y = center - int(number_radius * math.cos(angle))
        
        # 获取文字尺寸并居中
        text = str(number)
        bbox = draw.textbbox((0, 0), text, font=font)
        text_width = bbox[2] - bbox[0]
        text_height = bbox[3] - bbox[1]
        
        draw.text((x - text_width // 2, y - text_height // 2), text, 
                 fill=text_color, font=font)
    
    # 绘制时针 - 指向12点
    hour_width = 8
    hour_length = 60
    hour_color = (40, 40, 40, 255)
    
    for i in range(hour_width):
        offset = i - hour_width // 2
        draw.line([(center + offset, center), (center + offset, center - hour_length)], 
                 fill=hour_color, width=1)
    
    # 绘制分针 - 指向3点
    minute_width = 6
    minute_length = 80
    minute_color = (20, 20, 20, 255)
    
    for i in range(minute_width):
        offset = i - minute_width // 2
        draw.line([(center, center + offset), (center + minute_length, center + offset)], 
                 fill=minute_color, width=1)
    
    # 绘制中心圆点
    dot_radius = 12
    dot_color = (100, 100, 100, 255)
    dot_highlight = (220, 220, 220, 255)
    
    # 绘制圆点阴影
    draw.ellipse([center - dot_radius, center - dot_radius,
                  center + dot_radius, center + dot_radius], 
                 fill=dot_color)
    
    # 绘制圆点高光
    highlight_radius = dot_radius - 2
    draw.ellipse([center - highlight_radius, center - highlight_radius,
                  center + highlight_radius, center + highlight_radius], 
                 fill=dot_highlight)
    
    return img

def create_ico_file():
    """创建256x256高清ICO文件"""
    print("开始生成256x256高质量时钟图标...")
    
    img = create_256x256_clock_icon()
    
    # 保存为ICO文件
    ico_path = "定时关机软件.ico"
    
    try:
        img.save(ico_path, format='ICO', sizes=[(256, 256)])
        
        print(f"高质量图标已生成: {ico_path}")
        print(f"尺寸: 256x256")
        
        # 检查文件大小
        if os.path.exists(ico_path):
            file_size = os.path.getsize(ico_path)
            print(f"文件大小: {file_size} 字节 ({file_size/1024:.1f} KB)")
            
            if file_size > 5000:
                print("✓ 文件大小正常，应该很清晰！")
            else:
                print("⚠ 文件可能还是比较小，但应该比之前清晰")
        
    except Exception as e:
        print(f"保存ICO文件时出错: {e}")
        # 保存PNG备用
        png_path = "定时关机软件_256x256.png"
        img.save(png_path, "PNG")
        print(f"已保存PNG备用文件: {png_path}")

if __name__ == "__main__":
    create_ico_file() 